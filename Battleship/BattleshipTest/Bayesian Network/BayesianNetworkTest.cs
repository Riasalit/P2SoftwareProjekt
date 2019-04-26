using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using HAPI;
using System.Diagnostics;
using System.IO;
using BattleshipWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BattleshipTest.BayesianNetworkTest
{
    [TestClass]
    public class BayesianNetworkTest
    {
        [TestMethod]
        public void DoesAIWorkIfShipsArePlacedBesidesEachOther()
        {
            Player testAI = new AI("TestAI");
            Player testHuman = new AI("TestHuman");
            int i = 0;

            foreach (KeyValuePair<string, int> ship in Settings.ships)
            {
                testHuman.board.PlaceShips(new Ship(ship.Key, ship.Value, new Point(i++, 0), 'V'));
            }
            testAI.SetOpponent(testHuman);

            try
            {
                while (testHuman.board.sunkenShips != Settings.shipCount)
                {
                    testAI.YourTurn();
                }
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (ExceptionInconsistencyOrUnderflow e)
            {
                Debug.WriteLine(e.Message);
            }

            Assert.AreEqual(Settings.shipCount, testHuman.board.sunkenShips);
        }

    }
}
