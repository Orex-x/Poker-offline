package models

type PokerHand struct {
	Cards [2]string
}

func NewPokerHand() PokerHand {
	return PokerHand{
		Cards: [2]string{},
	}
}

func (ph *PokerHand) SetValue(i int, value string) {
	if i < len(ph.Cards) {
		ph.Cards[i] = value
	}
}
