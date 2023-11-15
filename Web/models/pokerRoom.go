package models

import (
	"math/rand"
)

type PokerRoom struct {
	Hands        map[string]PokerHand
	Table        []string
	Stack        *Stack
	cards        [52]string
	GameStarted  bool
	GameStages   []string
	GameStage    int
	OldGameStage int
	Pubsub       PubSub
}

func NewPokerRoom() *PokerRoom {
	return &PokerRoom{
		cards: [52]string{
			"s2", "s3", "s4", "s5", "s6", "s7", "s8", "s9", "s10", "sj0", "sq0", "sK0", "sa0",
			"h2", "h3", "h4", "h5", "h6", "h7", "h8", "h9", "h10", "hj0", "hq0", "hk0", "ha0",
			"c2", "c3", "c4", "c5", "c6", "c7", "c8", "c9", "c10", "cj0", "cq0", "ck0", "ca0",
			"d2", "d3", "d4", "d5", "d6", "d7", "d8", "d9", "d10", "dj0", "dq0", "dk0", "da0",
		},
		Table:        make([]string, 5),
		Hands:        make(map[string]PokerHand),
		Stack:        NewStack(),
		GameStarted:  false,
		GameStage:    0,
		OldGameStage: 0,
		GameStages:   []string{"Waiting", "Preflop", "Flop", "Turn", "River"},
		Pubsub:       *NewPubSub(),
	}
}

func (pr *PokerRoom) StartGame() (string, bool) {

	if len(pr.Hands) < 2 {
		return "For game need at least 2 players", false
	}

	if pr.GameStarted {
		return "The game is already underway", false
	}

	pr.GameStage = 1
	pr.GameStarted = true
	pr.ShuffleCard()
	pr.HandOutCards()

	return "", true
}

func (pr *PokerRoom) FinishGame() {
	pr.GameStage = 0
	pr.GameStarted = false
	pr.Stack.Clear()

	for k, l := range pr.Hands {
		l.Cards = [2]string{}
		pr.Hands[k] = l
	}

	pr.Table = make([]string, 5)
}

func (pr *PokerRoom) RestartGame() (string, bool) {

	if len(pr.Hands) < 2 {
		return "For game need at least 2 players", false
	}

	pr.GameStage = 1
	pr.GameStarted = true
	pr.Stack.Clear()
	pr.Table = make([]string, 5)

	pr.ShuffleCard()
	pr.HandOutCards()

	return "", true
}

func (pr *PokerRoom) ShuffleCard() {
	rand.Shuffle(len(pr.cards), func(i, j int) {
		pr.cards[i], pr.cards[j] = pr.cards[j], pr.cards[i]
	})

	tl := len(pr.Hands)*2 + 5

	for i := 0; i < tl; i++ {
		pr.Stack.Push(string(pr.cards[i]))
	}
}

func (pr *PokerRoom) Join(clientIP string) (string, bool) {
	if pr.GameStarted {
		return "The game is already underway", false
	}

	_, ok := pr.Hands[clientIP]

	if ok {
		return "You're already in the room", false
	}

	pr.Hands[clientIP] = NewPokerHand()
	return "", true
}

func (pr *PokerRoom) Exit(clientIP string) (string, bool) {
	if pr.GameStarted {
		return "The game is already underway", false
	}

	_, ok := pr.Hands[clientIP]

	if !ok {
		return "You're not in the room", false
	}

	delete(pr.Hands, clientIP)

	return "", true
}

func (pr *PokerRoom) GetTable() string {
	table := pr.Table
	result := ""

	for i := 0; i < 5; i++ {
		result += table[i] + " "
	}
	return result
}

func (pr *PokerRoom) HandOutCards() (string, bool) {

	if !pr.GameStarted {
		return "The game has not started" + pr.GameStages[pr.GameStage], false
	}

	for k, l := range pr.Hands {
		l.SetValue(0, pr.Stack.Pop())
		l.SetValue(1, pr.Stack.Pop())
		pr.Hands[k] = l
	}

	return "", true
}

func (pr *PokerRoom) Flop() (string, bool) {
	if !pr.GameStarted {
		return "The game has not started", false
	}

	if pr.GameStage != 1 {
		return "It is impossible to show Flop. Current stage of the game:" + pr.GameStages[pr.GameStage], false
	}

	for i := 0; i < 3; i++ {
		pr.Table[i] = pr.Stack.Pop()
	}

	pr.GameStage++
	return "", true
}

func (pr *PokerRoom) Turn() (string, bool) {
	if !pr.GameStarted {
		return "The game has not started", false
	}

	if pr.GameStage != 2 {
		return "It is impossible to show Turn. Current stage of the game:" + pr.GameStages[pr.GameStage], false
	}

	pr.Table[3] = pr.Stack.Pop()

	pr.GameStage++
	return "", true
}

func (pr *PokerRoom) River() (string, bool) {
	if !pr.GameStarted {
		return "The game has not started", false
	}

	if pr.GameStage != 3 {
		return "It is impossible to show River. Current stage of the game:" + pr.GameStages[pr.GameStage], false
	}

	pr.Table[4] = pr.Stack.Pop()

	pr.GameStage++
	return "", true
}
