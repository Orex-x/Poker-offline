package models

type Stack struct {
	data []string // посмотреть как реализовать через дженерики
}

func NewStack() *Stack {
	return &Stack{
		data: []string{},
	}
}

func (s *Stack) Push(item string) {
	s.data = append(s.data, item)
}

func (s *Stack) Pop() string {
	l := len(s.data)

	if l == 0 {
		panic("Stack is empty")
	}

	item := s.data[l-1]
	s.data = s.data[:l-1]
	return item
}

func (s *Stack) Clear() {
	s.data = []string{}
}
