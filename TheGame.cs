using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace Golf
{
    public class TheGame
    {
        public const double GRAVITY = 9.8;
        public int numberOfStrikes = 1;
        public Logger gameLog;
        private StrokeRecord _record;
        readonly Dictionary<int, string> StrikeTerms = new Dictionary<int, string> { { -3, "Albatross" }, { -2, "Eagle" }, { -1, "Birdie" }, { 0, "Par" }, { 1, "Bogey" }, { 2, "Double Bogey" }, { 3, "Tripple Bogey" } };
        public TheGame()
        {
            this.gameLog = new Logger();
            this._record = new StrokeRecord();
        }

        public void StartGame() {
            double _distance = 0, _distanceToHole = 0;
            bool ok = true;
            ParDistanceHole parDistance = EnterPar();
            StrokeData swing;
            _distanceToHole = parDistance.DistanceToHole();



            do
            {
                Console.WriteLine("\nYou have a distance of " + Math.Round(parDistance.DistanceToHole(),1).ToString()+" meters to the cup.");
                
                
                try
                {
                    if (numberOfStrikes > (parDistance.Par + 3))
                    {
                        throw new TooManyStrokesException("You had " + (numberOfStrikes-1).ToString() + " strikes on a par " + parDistance.Par.ToString() + " hole.");
                    }
                    else
                    {
                        swing = EnterSwing();
                        _distance = Distance(swing);
                        Console.WriteLine("Your swing was " + Math.Round(_distance, 1).ToString() + " meters long.");
                        if (Math.Round(_distanceToHole, 1)*2 < Math.Abs(Math.Round(_distanceToHole - _distance, 1)))
                        {
                            throw new TooFarFromCupException("Your last stroke for " + Math.Round(_distance, 1).ToString() + " meters took you more than twice as long from the cup as you were before the shot.");
                        }
                        else
                        {
                            this._record.Angel = swing.Angel;
                            this._record.Velocity = swing.Velocity;
                            this._record.DistanceToHole = parDistance.DistanceToHole();
                            this._record.StrikeDistance = _distance;
                            this._record.Strikes = numberOfStrikes;
                            this._record.ParOnHole = parDistance.Par;
                            this.gameLog.AddRecord(this._record);
                            numberOfStrikes++;
                        }
                    }

                    if (ok && Math.Round(_distance,1) > Math.Round(_distanceToHole,1))
                    {
                        Console.WriteLine("You overshot the hole by " + Math.Round((_distance - _distanceToHole),1).ToString() + " meters.");
                    }
                    else if (ok && Math.Round(_distance, 1) < Math.Round(_distanceToHole, 1))
                    {
                        
                    }
                    else
                    {
                        ok = false;
                        throw new SunkTheSwingException("Congratulation you scored a " + StrikeTerms[(numberOfStrikes - parDistance.Par)] + " on this hole.");
                    }
                    if (numberOfStrikes > 1)
                    {
                        Console.WriteLine("If you sink your next strike you will score a " + StrikeTerms[(numberOfStrikes - parDistance.Par)] + " on this hole");
                    }

                }
                catch (SunkTheSwingException ex)
                {
                    EndGame(ex.Message, false);
                    ok = false;
                }
                catch(TooManyStrokesException ex)
                {
                    EndGame(ex.Message, true);
                    ok = false;
                }
                catch(TooFarFromCupException ex)
                {
                    EndGame(ex.Message, true);
                    ok = false;
                }
                catch(Exception ex)
                {
                    EndGame(ex.Message, true);
                    ok = false;
                }
                _distanceToHole = parDistance.DistanceToHole(_distance);
            } while (ok);
        }

        public void EndGame(string data, bool error) {
            int _strikes = 0, _par = 0;
            Console.WriteLine("\n\nYour game has ended!");
            Console.WriteLine(data);
            Console.WriteLine("Your round looks like this:\n");

            foreach(StrokeRecord record in this.gameLog.StrikeRecord)
            {
                Console.WriteLine("Par:\t\t\t" + record.ParOnHole.ToString());
                Console.WriteLine("Strike:\t\t\t " + record.Strikes.ToString());
                Console.WriteLine("Angel:\t\t\t" + record.Angel.ToString()+" degrees.");
                Console.WriteLine("Velocity:\t\t" + record.Velocity.ToString()+"m/s.");
                Console.WriteLine("Distance:\t\t" + Math.Round(record.StrikeDistance,1).ToString()+" meters.");
                Console.WriteLine("Distance to hole:\t" + Math.Round(record.DistanceToHole,1).ToString()+" meters.");
                _strikes = record.Strikes;
                _par = record.ParOnHole;
                Console.WriteLine("\n------------------------------------\n");
            }
            if (_strikes < _par + 4)
            {
                if (_strikes == 1 && !error)
                {
                    Console.WriteLine("\n\nCongratulations you scored a Hole-In-One!!!!!");
                }
                else
                {
                    if (!error)
                    {
                        Console.WriteLine("\nYou scored a " + StrikeTerms[_strikes - _par] + " on this hole!");
                    }
                }
            }
        }

        private ParDistanceHole EnterPar()
        {
            string _strPar = "";
            Console.Write("Enter what 'par' your tee off will have (3, 4 or 5): ");
            _strPar = Console.ReadLine();
            Console.WriteLine("");
            if (IsNumber(_strPar))
            {
                try
                {
                    return new ParDistanceHole(Convert.ToInt32(_strPar));
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Out of range data.", "_strPar", ex);
                }
            }
            else
            {
                throw new ArgumentException("Input non numerical.", "_strPar");
            }
        }

        private StrokeData EnterSwing()
        {
            string _strVel = "", _strAngl = "";

            Console.Write("Enter force/velocity of swing in m/s: ");
            _strVel = Console.ReadLine();
            Console.Write("Enter initial angel in degrees: ");
            _strAngl = Console.ReadLine();

            if (IsNumber(_strVel) && IsNumber(_strAngl))
            {
                return new StrokeData(Convert.ToDouble(_strVel), Convert.ToDouble(_strAngl));
            }
            else
            {
                throw new FormatException("Entered value not numerical.");
            }
        }

        private double Distance(StrokeData data)
        {
            double _rad = (Math.PI / 180) * data.Angel;

            return Math.Pow(data.Velocity, 2) * Math.Sin(2 * _rad) / GRAVITY;
        }

        public bool IsNumber(string data)
        {
            string pattern = @"[^0-9\.\-]";
            Regex rgx = new Regex(pattern);
            MatchCollection matches = rgx.Matches(data);
            if (matches.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }


    public struct StrokeData : IStrokeData
    {
        public StrokeData(double velocity, double angel)
        {
            Velocity = velocity;
            Angel = angel;
        }

        public double Velocity { get; }
        public double Angel { get; }

    }
    public interface IStrokeData
    {
        double Velocity { get; }
        double Angel { get; }
    }
    public interface IStrokeRecord : IStrokeData
    {
        int Strikes { get; set; }
        int ParOnHole { get; set; }
        double StrikeDistance { get; set; }
        double DistanceToHole { get; set; }
    }

    public struct StrokeRecord : IStrokeRecord
    {
        public double Velocity { get; set; }
        public double Angel { get; set; }
        public int Strikes { get; set; }
        public int ParOnHole { get; set; }
        public double StrikeDistance { get; set; }
        public double DistanceToHole { get; set; }
    }

    public class ParDistanceHole
    {
        private double _distanceToHole;
        public ParDistanceHole(int par)
        {
            Par = par;
            this._distanceToHole = Distance;
        }
        public int Par { get; }
        public double Distance
        {
            get
            {
                if (Par == 3) return 170.3;
                else if (Par == 4) return 416.5;
                else if (Par == 5) return 499.5;
                else
                {
                    throw new ArgumentException("Input argument out of range. Valid range 3, 4, 5", "Par");
                }
            }
        }

        public double DistanceToHole(double shotLength)
        {
            this._distanceToHole = Math.Abs(this._distanceToHole - shotLength);
            return this._distanceToHole;
        }
        public double DistanceToHole()
        {
            return this._distanceToHole;
        }
    }
    [Serializable]
    public class SunkTheSwingException : Exception
    {
        public SunkTheSwingException() : base() { }
        public SunkTheSwingException(string message) : base(message) { }
        public SunkTheSwingException(string message, Exception inner) : base(message, inner) { }
    }
    [Serializable]
    public class TooManyStrokesException : Exception
    {
        public TooManyStrokesException() : base() { }
        public TooManyStrokesException(string message) : base(message) { }
        public TooManyStrokesException(string message, Exception inner) : base(message, inner) { }
    }
    [Serializable]
    public class TooFarFromCupException : Exception
    {
        public TooFarFromCupException() : base() { }
        public TooFarFromCupException(string message) : base(message) { }
        public TooFarFromCupException(string message, Exception inner) : base(message, inner) { }
    }
}

