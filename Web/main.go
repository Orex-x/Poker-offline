package main

import (
	"fmt"
	"log"
	"net"
	"net/http"
	"pokerOffline/handlers"
	"time"

	"github.com/julienschmidt/httprouter"
)

func main() {



	fmt.Println("hi")
	router := httprouter.New()

	game := handlers.NewGameHandler()
	game.Register(router)

	start(router)
}

func start(router *httprouter.Router) {
	listener, err := net.Listen("tcp", "0.0.0.0:8080")

	if err != nil {
		panic(err)
	}

	server := &http.Server{
		Handler:      router,
		WriteTimeout: 15 * time.Minute,
		ReadTimeout:  15 * time.Minute,
	}

	log.Fatalln(server.Serve(listener))
}
