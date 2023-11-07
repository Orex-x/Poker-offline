package handlers

import (
	"github.com/julienschmidt/httprouter"
)

type BaseHandler interface {
	Register(router *httprouter.Router)
}
