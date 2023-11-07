
## API Reference

#### Create room

```
  GET /createRoom?{name}
```

Сreates the room with a given name, the room is an analogue of the table at which the players play.

#### Join room

```
  GET /joinRoom?{name}
```

Joining the room by a given name. 

#### Exit room

```
  GET /exitRoom?{name}
```

Exit the room under the given name. 

#### Start game

```
  GET /startGame?{name}
```

Starts the game if there are at least two players in the room. The cards are shuffled, dealt to the players and prepared to appear on the table.


#### Restart game

```
  GET /restartGame?{name}
```

А new game starts if there are at least two players in the room. The cards are shuffled, dealt to the players and prepared to appear on the table.


#### Finish game

```
  GET /finishGame?{name}
```

The game ends, the hands and the table are cleared.

#### Get hand

```
  GET /getHand?{name}
```

Returns the player's cards in the room.

#### Get table

```
  GET /getTable?{name}
```

Returns the cards on the table.

#### Reveal the flop

```
  GET /flop?{name}
```

Opens the flop on the table.

#### Reveal the turn

```
  GET /turn?{name}
```

Opens the turn on the table.


#### Reveal the river

```
  GET /river?{name}
```

Opens the river on the table.


## Authors

- [@orex](https://t.me/OreX_x)
