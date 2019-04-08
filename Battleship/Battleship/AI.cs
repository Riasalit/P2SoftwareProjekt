using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using HAPI;

namespace Battleship
{
    public class AI : Player
    {
        public AI(string name) : base(name)
        {

        }
        public override void YourTurn()
        {
            InitBayesianNetwork();
        }
        private Domain InitBayesianNetwork()
        {
            Domain battleShip = new Domain();
            NodeList shipList = new NodeList();
            string[] shipNames = new string[5] {"Destroyer", "Submarine",
                                                "Cruiser", "Battleship", "Carrier"};
            int[] shipLengths = new int[5] { 2, 3, 3, 4, 5 };
            //Initializes ships
            for (int i = 0; i < 5; i++)
            {
                shipList.Add(new LabelledDCNode(battleShip));
                shipList[i].SetName(shipNames[i]);
                shipList[i].SetLabel(shipNames[i]);
                SetAllStatesForShips(shipList[i], shipLengths[i]);    
            }
            //Initializes constraints (4+3+2+1=10 constraints)
            for (int i = 0; i < 5; i++)
            {
                for (int j = i+1; j < 5; j++)
                {
                    BooleanDCNode constraint = new BooleanDCNode(battleShip);
                    constraint.AddParent(shipList[i]);
                    constraint.AddParent(shipList[j]);
                }
            }
            
            //Initializes tiles
            for (int i = 0; i < 10; i++)
            {
                char letter = (char)(i + 65);
                for(int j = 0; j < 10; j++)
                {
                    LabelledDCNode tile = new LabelledDCNode(battleShip);
                    tile.SetName(letter + $"{(j + 1)}");
                    tile.SetLabel(letter + $"{(j + 1)}");
                    for (int k = 0; k < 5; k++)
                    {
                        tile.AddParent(shipList[k]);
                    }
                }
            }
            //We lost here
            //B.SetNumberOfStates(4);
            //for (uint i = 0; i < 4; i++)
            //{
            //    B.SetStateLabel(i, "state " + i);
            //}
            ////make A parent of B (A->B) 
            //B.AddParent(A);
            //Table tableB = B.GetTable();
            //for (uint i = 0; i < tableB.GetSize(); i++)
            //{
            //    tableB.SetDataItem(i, i + 1);
            //}
            return battleShip;
              
        }
        private void SetAllStatesForShips(Node node, int length)
        {

        }
        public override void SetShips()
        {
            int orientation;
            string[] shipNames = new string[5] {"Destroyer", "Submarine",
                                                "Cruiser", "Battleship", "Carrier"};
            int[] shipLengths = new int[5] { 2, 3, 3, 4, 5 };

            for (int i = 0; i < 5; i++)
            {
                bool correctlyPlaced = false;
                while (!correctlyPlaced)
                {
                    Point point = new Point();
                    point.X = new Random().Next(0, 10);
                    point.Y = new Random().Next(0, 10);

                    orientation = new Random().Next(0, 2);
                    correctlyPlaced = board.PlaceShips(new Ship(shipNames[i], shipLengths[i], point,
                                                       orientation == 0 ? 'H' : 'V'));
                }
            }
        }
    }

}
