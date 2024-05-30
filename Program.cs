namespace Lessons21
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ComputerClub computerClub = new(5);
            computerClub.Work();
        }
    }
    public class ComputerClub
    {
        private int _money = 0;
        private List<Computer> _computers = new List<Computer>();
        private Queue<Client> _clients = new Queue<Client>();
        public ComputerClub(int computersCount)
        {
            Random random = new Random();

            for (int i = 0; i < computersCount; i++)
            {
                _computers.Add(new Computer(random.Next(5, 15)));
            }
            NewClients(25, random);
        }
        public void NewClients(int clientsCount, Random random)
        {
            for (int i = 0; i < clientsCount; i++)
            {
                _clients.Enqueue(new Client(random.Next(100, 250), random));
            }
        }
        public void Work()
        {
            while (_clients.Count > 0)
            {
                Client newClient = _clients.Dequeue();
                Console.WriteLine($"Club`s balance: {_money}. Waiting for a new client.");
                Console.WriteLine($"New client is arrived! He desires to buy {newClient.DesiredMinutes} minutes.\n");

                ShowAllComputerStates();
                Console.Write("Propose computer of number: ");
                string userInput = Console.ReadLine();

                if (int.TryParse(userInput, out int computerNumber))
                {
                    computerNumber--;
                    if (ComputerExistsAndFree(computerNumber))
                    {
                        if (_computers[computerNumber].IsTaken)
                        {
                            Console.WriteLine("Computer is already taken");
                        }
                        else
                        {
                            if (newClient.CheckSolvency(_computers[computerNumber]))
                            {
                                Console.WriteLine($"Client paid and took a computer number {computerNumber + 1}");
                                _money += newClient.Pay();
                                _computers[computerNumber].BecomeTaken(newClient);
                            }
                            else
                            {
                                Console.WriteLine("Client have not enough money");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unknown computer");
                    }
                }
                else
                {
                    NewClients(1, new Random());
                    Console.WriteLine("Wrong number");
                }

                Console.WriteLine("Press any key, to serve another client");
                Console.ReadKey();
                Console.Clear();
                SpendOneMinute();
            }
        }
        private void ShowAllComputerStates()
        {
            Console.WriteLine("All computers list: ");
            for (int i = 0; i < _computers.Count; i++)
            {
                Console.Write(i + 1 + ". ");
                _computers[i].ShowState();
            }
        }
        private bool ComputerExistsAndFree(int computerNumber)
        {
            return computerNumber >= 0 && computerNumber < _computers.Count;
        }
        private void SpendOneMinute()
        {
            foreach (var computer in _computers)
            {
                computer.SpendOneMinute();
            }
        }
    }

    public class Computer
    {
        private int _minutesRemaining;
        public Client _client { get; private set; }
        public int PricePerMinute { get; private set; }
        public bool IsTaken
        {
            get
            {
                return _minutesRemaining > 0;
            }
        }

        public Computer(int pricePerMinute)
        {
            PricePerMinute = pricePerMinute;
        }

        public void BecomeTaken(Client client)
        {
            _client = client;
            _minutesRemaining = _client.DesiredMinutes;
        }
        public void BecomeEmpty()
        {
            _client = null;
        }
        public void SpendOneMinute()
        {
            _minutesRemaining--;
        }
        public void ShowState()
        {
            if (IsTaken)
                Console.WriteLine($"Computer is taken, minutes remaining: {_minutesRemaining}");
            else
                Console.WriteLine($"Computer is free, price per minute is: {PricePerMinute}");
        }
    }

    public class Client
    {
        private int _money;
        private int _moneyToPay;
        public int DesiredMinutes { get; private set; }

        public Client(int money, Random random)
        {
            _money = money;
            DesiredMinutes = random.Next(10, 31);
        }
        public bool CheckSolvency(Computer computer)
        {
            _moneyToPay = DesiredMinutes * computer.PricePerMinute;
            if (_money >= _moneyToPay)
                return true;
            else
            {
                _moneyToPay = 0;
                return false;
            }
                
        }
        public int Pay()
        {
            _money -= _moneyToPay;
            return _moneyToPay;
        }
    }
}
