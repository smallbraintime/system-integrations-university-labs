FROM golang:1.21 AS builder

WORKDIR /app

COPY go.mod ./
RUN go mod download

COPY . .

RUN CGO_ENABLED=0 go build -o myapp

FROM alpine:latest

WORKDIR /root/

COPY --from=builder /app/myapp .

COPY service.wsdl .

RUN chmod +x /root/myapp

EXPOSE 8080

CMD ["./myapp"]
