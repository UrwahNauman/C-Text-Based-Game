using System;
using System.Collections.Generic;

// Player class representing the main character
class Player
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int Gold { get; set; }
    public List<Item> Inventory { get; set; }

    public Player(string name)
    {
        Name = name;
        Health = 100; // starting health
        Gold = 0; // starting gold
        Inventory = new List<Item>();
    }

    public void AddToInventory(Item item)
    {
        Inventory.Add(item);
    }

    public bool HasItem(string itemName)
    {
        return Inventory.Exists(item => item.Name.ToLower() == itemName.ToLower());
    }

    public void DisplayInventory()
    {
        Console.WriteLine($"Inventory of {Name}:");
        foreach (var item in Inventory)
        {
            Console.WriteLine($"- {item.Name}: {item.Description}");
        }
        Console.WriteLine($"Gold: {Gold}");
    }
}

// Item class representing items in the game
class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Value { get; set; } // Gold value of the item

    public Item(string name, string description, int value)
    {
        Name = name;
        Description = description;
        Value = value;
    }
}

// Room class representing locations in the game world
class Room
{
    public string Description { get; set; }
    public Dictionary<string, Room> Exits { get; set; }
    public List<Item> Items { get; set; }
    public Enemy Enemy { get; set; }

    public Room(string description)
    {
        Description = description;
        Exits = new Dictionary<string, Room>();
        Items = new List<Item>();
        Enemy = null;
    }

    public void AddItem(Item item)
    {
        Items.Add(item);
    }
}

// Enemy class representing enemies in the game
class Enemy
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int AttackPower { get; set; }
    public int GoldReward { get; set; }

    public Enemy(string name, int health, int attackPower, int goldReward)
    {
        Name = name;
        Health = health;
        AttackPower = attackPower;
        GoldReward = goldReward;
    }
}

// GameEngine class managing game flow, interactions, and logic
class GameEngine
{
    private Player player;
    private Dictionary<string, Room> worldMap;
    private Room currentRoom;

    public GameEngine(Player player, Dictionary<string, Room> worldMap)
    {
        this.player = player;
        this.worldMap = worldMap;
        currentRoom = worldMap["start"]; // set starting room
    }

    public void Start()
    {
        Console.WriteLine("Hail Adventure Awaits!");
        Console.WriteLine("Explore the dark world and interact with the enemies.");

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine(currentRoom.Description);

            // Display available exits
            Console.Write("Exits: ");
            foreach (var exit in currentRoom.Exits.Keys)
            {
                Console.Write(exit + " ");
            }
            Console.WriteLine();

            // Handle player input
            Console.Write("Choose your direction (or type 'quit' to exit): ");
            string input = Console.ReadLine().ToLower();

            if (input == "quit")
            {
                Console.WriteLine("Thanks for playing!");
                break;
            }
            else if (currentRoom.Exits.ContainsKey(input))
            {
                currentRoom = currentRoom.Exits[input];
            }
            else
            {
                Console.WriteLine("Invalid direction!");
            }

            // Check for items in the current room
            if (currentRoom.Items.Count > 0)
            {
                Console.WriteLine("You see the following items in the room:");
                foreach (var item in currentRoom.Items)
                {
                    Console.WriteLine($"- {item.Name}: {item.Description}");
                }

                Console.Write("Enter item name to take (or 'skip' to continue): ");
                string itemChoice = Console.ReadLine().ToLower();

                if (currentRoom.Items.Exists(item => item.Name.ToLower() == itemChoice))
                {
                    Item item = currentRoom.Items.Find(item => item.Name.ToLower() == itemChoice);
                    player.AddToInventory(item);
                    player.Gold += item.Value; // add item value to gold
                    currentRoom.Items.Remove(item);
                    Console.WriteLine($"You took the {item.Name}.");
                }
                else if (itemChoice != "skip")
                {
                    Console.WriteLine("Invalid item choice.");
                }
            }

            // Handle combat with enemy if present
            if (currentRoom.Enemy != null)
            {
                Console.WriteLine($"You encounter a {currentRoom.Enemy.Name} with {currentRoom.Enemy.Health} health!");

                // Example: Simple combat mechanics
                while (currentRoom.Enemy.Health > 0 && player.Health > 0)
                {
                    Console.WriteLine($"Your health: {player.Health}");
                    Console.WriteLine($"Enemy's health: {currentRoom.Enemy.Health}");

                    Console.Write("Choose your action (fight/flee): ");
                    string action = Console.ReadLine().ToLower();

                    if (action == "fight")
                    {
                        // Example: Combat resolution
                        int playerDamage = new Random().Next(5, 15);
                        int enemyDamage = new Random().Next(3, 10);

                        currentRoom.Enemy.Health -= playerDamage;
                        player.Health -= enemyDamage;

                        Console.WriteLine($"You attack the {currentRoom.Enemy.Name} for {playerDamage} damage.");
                        Console.WriteLine($"The {currentRoom.Enemy.Name} attacks you for {enemyDamage} damage.");

                        if (currentRoom.Enemy.Health <= 0)
                        {
                            Console.WriteLine($"You defeated the {currentRoom.Enemy.Name}!");
                            player.Gold += currentRoom.Enemy.GoldReward; // reward gold for defeating enemy
                            currentRoom.Enemy = null; // Remove enemy from room
                            break;
                        }
                        else if (player.Health <= 0)
                        {
                            Console.WriteLine("You have been defeated...");
                            break;
                        }
                    }
                    else if (action == "flee")
                    {
                        Console.WriteLine("You managed to escape from the enemy!");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid action. The enemy attacks!");
                        player.Health -= new Random().Next(3, 10); // Example: Enemy attacks
                    }
                }
            }

            // Display player's inventory and stats
            player.DisplayInventory();
        }
    }
}

// Main program entry point
class Program
{
    static void Main(string[] args)
    {
        // Initialize player
        Player player = new Player("Hero");

        // Initialize world map and rooms
        Dictionary<string, Room> worldMap = new Dictionary<string, Room>();
        Room startRoom = new Room("You are in a dark forest. Paths lead north and east.");
        Room northRoom = new Room("You find yourself in a clearing with a lake to the north.");
        Room eastRoom = new Room("You come across an abandoned cabin. Nothing there!");

        // Connect rooms
        startRoom.Exits["north"] = northRoom;
        startRoom.Exits["east"] = eastRoom;
        northRoom.Exits["south"] = startRoom;
        eastRoom.Exits["west"] = startRoom;

        // Add items and enemies
        startRoom.Items.Add(new Item("sword", "A rusty sword lies on the ground.", 20));
        northRoom.Enemy = new Enemy("Goblin", 20, 5, 10); // Example enemy with gold reward

        // Add rooms to world map
        worldMap["start"] = startRoom;
        worldMap["north"] = northRoom;
        worldMap["east"] = eastRoom;

        // Start the game engine
        GameEngine gameEngine = new GameEngine(player, worldMap);
        gameEngine.Start();
    }
}
