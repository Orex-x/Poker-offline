package handlers

import (
	"net/http"
	"pokerOffline/models"
	"strings"

	"github.com/julienschmidt/httprouter"
)

type GameHandler struct {
	rooms map[string]*models.PokerRoom
}

func (h *GameHandler) Register(router *httprouter.Router) {
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
}

func NewGameHandler() BaseHandler {
	return &GameHandler{
		rooms: make(map[string]*models.PokerRoom),
	}
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
