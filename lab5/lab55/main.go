package main

import (
	"fmt"
	"io"
	"log"
	"net/http"
	"os"
	"strings"
)

const GetClientsTemplate = `<?xml version="1.0"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
	<soap:Body>
		<getAllClientsResponse xmlns="http://example.com/clientService">
			<clients>%s</clients>
		</getAllClientsResponse>
	</soap:Body>
</soap:Envelope>`

const AddClientResponse = `<?xml version="1.0"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
	<soap:Body>
		<addClientResponse xmlns="http://example.com/clientService"/>
	</soap:Body>
</soap:Envelope>`

func wsdlHandler(w http.ResponseWriter, r *http.Request) {
	log.Printf("Received wsdl request")

	if r.Method == http.MethodGet {
		w.Header().Set("Content-Type", "text/xml; charset=utf-8")

		wsdl, err := os.ReadFile("service.wsdl")
		if err != nil {
			http.Error(w, "WSDL file not found", http.StatusInternalServerError)
		}
		w.Write(wsdl)
	}
}

var clients = []string{"Bob", "Bill"}

func clientHandler(w http.ResponseWriter, r *http.Request) {
	log.Printf("Received client request")

	if r.Method != http.MethodPost {
		http.Error(w, "Only POST method allowed", http.StatusInternalServerError)
		return
	}

	body, err := io.ReadAll(r.Body)
	if err != nil {
		http.Error(w, "Failed to read request body", http.StatusInternalServerError)
		return
	}
	defer r.Body.Close()

	var response string

	if strings.Contains(string(body), "getAllClients") {
		clientList := strings.Join(clients, ",")
		response = fmt.Sprintf(GetClientsTemplate, clientList)
	}

	if strings.Contains(string(body), "addClient") {
		start := strings.Index(string(body), "<name>") + len("<name>")
		end := strings.Index(string(body), "</name>")
		if start > len("<name>")-1 && end > start {
			newClient := string(body[start:end])
			clients = append(clients, newClient)
			log.Printf("New client added: %s", newClient)
		}
		response = AddClientResponse
	}

	if response == "" {
		http.Error(w, "Invalid SOAP request", http.StatusBadRequest)
		return
	}

	w.Header().Set("Content-Type", "text/xml; charset=utf-8")
	w.WriteHeader(http.StatusOK)
	_, _ = w.Write([]byte(response))
}

func main() {
	http.HandleFunc("/client", clientHandler)
	http.HandleFunc("/wsdl", wsdlHandler)
	fmt.Println("SOAP Server listening on :8080...")
	http.ListenAndServe(":8080", nil)
}
