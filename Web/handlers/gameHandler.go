package handlers

import (
	"encoding/json"
	"net/http"
	"pokerOffline/models"
	"strings"

	"github.com/julienschmidt/httprouter"
)

type GameHandler struct {
	rooms map[string]*models.PokerRoom
}

func (h *GameHandler) Register(router *httprouter.Router) {
	router.GET("/rooms", h.ListRooms)
	router.GET("/createRoom", h.CrateRoom)
	router.GET("/joinRoom", h.JoinRoom)
	router.GET("/exitRoom", h.ExitRoom)
	router.GET("/startGame", h.StartGame)
	router.GET("/restartGame", h.RestartGame)
	router.GET("/finishGame", h.FinishGame)
	router.GET("/getHand", h.GetHand)
	router.GET("/getTable", h.GetTable)
	router.GET("/flop", h.Flop)
	router.GET("/turn", h.Turn)
	router.GET("/river", h.River)
	router.GET("/getStatus", h.GetStatus) //long pooling
}

func NewGameHandler() BaseHandler {
	return &GameHandler{
		rooms: make(map[string]*models.PokerRoom),
	}
}

func (h *GameHandler) GetStatus(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	name := r.URL.Query().Get("name")

	if name == "" {
		http.Error(w, "The 'name' or 'status' parameters is missing", http.StatusBadRequest)
		return
	}
	room, ok := h.rooms[name]

	if !ok {
		http.Error(w, "The room doesn't exist", http.StatusBadRequest)
		return
	}

	eventName, close := room.Pubsub.Subscribe()
	defer close()

	select {
	case eventValue := <-eventName:
		w.Write([]byte(eventValue))
		return
	case <-r.Context().Done():
		w.Write([]byte("timeout"))
		return
	}
}

func (h *GameHandler) ListRooms(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	list := []string{}

	for k := range h.rooms {
		list = append(list, k)
	}

	j, err := json.Marshal(list)

	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	w.Write([]byte(j))
}

func (h *GameHandler) CrateRoom(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	name := r.URL.Query().Get("name")

	if name == "" {
		http.Error(w, "The 'name' parameter is missing", http.StatusBadRequest)
		return
	}
	_, ok := h.rooms[name]

	if ok {
		http.Error(w, "A room with that name already exists", http.StatusBadRequest)
		return
	}

	h.rooms[name] = models.NewPokerRoom()

	w.Write([]byte("You have created a room!"))
}

func (h *GameHandler) JoinRoom(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	clientIP := strings.Split(r.RemoteAddr, ":")[0]

	name := r.URL.Query().Get("name")

	pokerRoom, err := h.GetPokerRoom(name)

	if err != "" {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	err, ok := pokerRoom.Join(clientIP)

	if !ok {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	w.Write([]byte("You entered the room!"))
}

func (h *GameHandler) ExitRoom(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	clientIP := strings.Split(r.RemoteAddr, ":")[0]

	name := r.URL.Query().Get("name")

	pokerRoom, err := h.GetPokerRoom(name)

	if err != "" {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	err, ok := pokerRoom.Exit(clientIP)

	if !ok {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	w.Write([]byte("Successfully exited the group!"))
}

func (h *GameHandler) StartGame(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	name := r.URL.Query().Get("name")

	pokerRoom, err := h.GetPokerRoom(name)

	if err != "" {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	err, ok := pokerRoom.StartGame()

	if !ok {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	w.Write([]byte("Game started!"))
	pokerRoom.Pubsub.Publish("StartGame")
}

func (h *GameHandler) RestartGame(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	name := r.URL.Query().Get("name")

	pokerRoom, err := h.GetPokerRoom(name)

	if err != "" {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	err, ok := pokerRoom.RestartGame()

	if !ok {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	w.Write([]byte("Game restarted!"))
	pokerRoom.Pubsub.Publish("RestartGame")
}

func (h *GameHandler) FinishGame(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	name := r.URL.Query().Get("name")

	pokerRoom, err := h.GetPokerRoom(name)

	if err != "" {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	pokerRoom.FinishGame()
	w.Write([]byte("Game finish!"))
	pokerRoom.Pubsub.Publish("FinishGame")
}

func (h *GameHandler) GetHand(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	clientIP := strings.Split(r.RemoteAddr, ":")[0]

	name := r.URL.Query().Get("name")

	pokerRoom, err := h.GetPokerRoom(name)

	if err != "" {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	hand, ok := pokerRoom.Hands[clientIP]

	if !ok {
		http.Error(w, "You are not in this room", http.StatusBadRequest)
		return
	}

	w.Write([]byte(hand.Cards[0] + " " + hand.Cards[1]))
}

func (h *GameHandler) GetTable(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	name := r.URL.Query().Get("name")

	pokerRoom, err := h.GetPokerRoom(name)

	if err != "" {
		http.Error(w, err, http.StatusBadRequest)
		return
	}
	result := pokerRoom.GetTable()

	w.Write([]byte(result))
}

func (h *GameHandler) Flop(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	name := r.URL.Query().Get("name")

	pokerRoom, err := h.GetPokerRoom(name)

	if err != "" {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	err, ok := pokerRoom.Flop()

	if !ok {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	w.Write([]byte("Flop is shown"))
	pokerRoom.Pubsub.Publish("Flop")
}

func (h *GameHandler) Turn(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	name := r.URL.Query().Get("name")

	pokerRoom, err := h.GetPokerRoom(name)

	if err != "" {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	err, ok := pokerRoom.Turn()

	if !ok {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	w.Write([]byte("Turn is shown"))
	pokerRoom.Pubsub.Publish("Turn")
}

func (h *GameHandler) River(w http.ResponseWriter, r *http.Request, params httprouter.Params) {
	name := r.URL.Query().Get("name")

	pokerRoom, err := h.GetPokerRoom(name)

	if err != "" {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	err, ok := pokerRoom.River()

	if !ok {
		http.Error(w, err, http.StatusBadRequest)
		return
	}

	w.Write([]byte("River is shown"))
	pokerRoom.Pubsub.Publish("River")
}

func (h *GameHandler) GetPokerRoom(name string) (*models.PokerRoom, string) {

	if name == "" {
		return &models.PokerRoom{}, "The 'name' parameter is missing"
	}

	pokerRoom, ok := h.rooms[name]

	if !ok {
		return &models.PokerRoom{}, "There is no room with that name"
	}

	return pokerRoom, ""
}
