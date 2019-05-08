using System;
using System.Collections.Generic;
using System.Drawing;

namespace BattleshipWeb
{
    public abstract class Player
    {
        public List<Point> pointsShot;
        public Board board { get; }
        private Player opponent;
        public string playerName { get; }
        public int turnCounter { get; private set; }
        public bool turnFailed;
        public static Random random;

        public Player(string name)
        {
            playerName = name;
            board = new Board();
            pointsShot = new List<Point>();
            turnCounter = 0;
            turnFailed = false;
            random = new Random();
        }
        public void SetOpponent(Player opponent)
        {
            this.opponent = opponent;
        }
        public Tile ShootOpponent(Point point)
        {
            turnCounter++;

            return opponent.board.ShootAt(point);
        }
        public abstract void YourTurn();
        public abstract void SetShips();

        public void SetMidStackShip()
        {
            board.PlaceShips(new Ship("Battleship", 4, new Point(2, 2), 'H'));
            board.PlaceShips(new Ship("Cruiser", 3, new Point(3, 3), 'V'));
            board.PlaceShips(new Ship("Submarine", 3, new Point(4, 3), 'V'));
        }
        public void SetCornerStackShip()
        {
            board.PlaceShips(new Ship("Battleship", 4, new Point(0, 0), 'H'));
            board.PlaceShips(new Ship("Cruiser", 3, new Point(0, 1), 'H'));
            board.PlaceShips(new Ship("Submarine", 3, new Point(0, 2), 'H'));
        }
        public void SetCornerSpreadShip()
        {
            board.PlaceShips(new Ship("Battleship", 4, new Point(0, 0), 'H'));
            board.PlaceShips(new Ship("Cruiser", 3, new Point(7, 0), 'V'));
            board.PlaceShips(new Ship("Submarine", 3, new Point(0, 7), 'H'));
        }
        public void SetBorderStackShip()
        {
            board.PlaceShips(new Ship("Battleship", 4, new Point(0, 7), 'H'));
            board.PlaceShips(new Ship("Cruiser", 3, new Point(4, 7), 'H'));
            board.PlaceShips(new Ship("Submarine", 3, new Point(7, 5), 'V'));
        }
        public void SetMidSpreadShip()
        {
            board.PlaceShips(new Ship("Battleship", 4, new Point(2, 1), 'H'));
            board.PlaceShips(new Ship("Cruiser", 3, new Point(6, 3), 'V'));
            board.PlaceShips(new Ship("Submarine", 3, new Point(1, 4), 'V'));
        }
    }
}
