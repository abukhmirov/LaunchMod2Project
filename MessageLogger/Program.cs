using MessageLogger.Data;
using Microsoft.EntityFrameworkCore;



// LEAVE THE WRITELINES TO PRESERVE THE UX
Console.WriteLine("Welcome to Message Logger!");
Console.WriteLine();
Console.WriteLine("Let's create a user pofile for you.");
// NEW USER CREATION
Console.Write("What is your name? ");
string name = Console.ReadLine();
Console.Write("What is your username? (one word, no spaces!) ");
string username = Console.ReadLine();

using (var context = new MessageLoggerContext())
{
    User user = new User(name, username);

    context.Users.Add(user);
    context.SaveChanges();
    user = context.Users.Single(u => u.Username == username);

    // WRITE USER TO DATABASE

    Console.WriteLine();
    Console.WriteLine("To log out of your user profile, enter `log out`.");
    Console.WriteLine();
    Console.Write("Add a message (or `quit` to exit): ");

    // MESSAGE CREATION AND ADDING TO MESSAGES LIST IN USER

    string userInput = Console.ReadLine();

    List<User> users = new List<User>() { user };

    while (userInput.ToLower() != "quit")
    {
        while (userInput.ToLower() != "log out")
        {
            // WRITE MESSAGE TO DATABASE

            //context.Messages.Add(new Message(userInput));
            user.Messages.Add(new Message(userInput));
            context.SaveChanges();

            foreach (var message in user.Messages)
            {
                Console.WriteLine($"{user.Name} {message.CreatedAt:t}: {message.Content}");
            }

            Console.Write("Add a message: ");

            userInput = Console.ReadLine();
            Console.WriteLine();

        }

        Console.Write("Would you like to log in a `new` or `existing` user? Or, `quit`? ");
        userInput = Console.ReadLine();
        if (userInput.ToLower() == "new")
        {
            Console.Write("What is your name? ");

            name = Console.ReadLine();

            Console.Write("What is your username? (one word, no spaces!) ");

            username = Console.ReadLine();

            user = new User(name, username);

            users.Add(user);

            // WRITE USER INTO DATABASE

            context.Users.Add(user);
            context.SaveChanges();

            user = context.Users.Single(u => u.Username == username);


            Console.Write("Add a message: ");

            userInput = Console.ReadLine();

        }
        else if (userInput.ToLower() == "existing")
        {
            Console.Write("What is your username? ");
            username = Console.ReadLine();
            user = null;
            foreach (var existingUser in context.Users.Include(u => u.Messages))
            {
                if (existingUser.Username == username)
                {
                    user = existingUser;
                }
            }

            if (user != null)
            {
                Console.Write("Add a message: ");
                userInput = Console.ReadLine();
            }
            else
            {
                Console.WriteLine("could not find user");
                userInput = "quit";

            }
        }

    }

    Console.WriteLine("Thanks for using Message Logger!");
    // PULL MESSAGE COUNT AND USERS FROM DATABASE

    var usersOrderedByMessageCount = context.Users.Include(u => u.Messages)
    .OrderByDescending(u => u.Messages.Count);

    Console.WriteLine();
    Console.WriteLine("Users ordered by number of messages created (most to least):");
    foreach (var u in usersOrderedByMessageCount)
    {
        Console.WriteLine($"{u.Name} wrote {u.Messages.Count} messages.");
    }


    var mostCommonMessages = context.Messages
     .GroupBy(m => m.Content)
     .OrderByDescending(g => g.Count())
     .Select(g => new { Message = g.Key, Count = g.Count() })
     .ToList();


    // Find the most common message by user
    var mostCommonMessagesByUser = context.Users.Include(u => u.Messages)
        .Select(u => new
        {
            UserName = u.Username,
            MostCommonMessage = u.Messages
                .GroupBy(m => m.Content) 
                .OrderByDescending(g => g.Count()) 
                .Select(g => new { Message = g.Key, Count = g.Count() }) 
                .FirstOrDefault() 
        })
        .Where(u => u.MostCommonMessage != null) 
        .ToList();

   // Messages by Hour to use for the hour with most messages
    var messagesByHour = context.Messages
    .GroupBy(m => m.CreatedAt.Hour)
    .Select(g => new { Hour = g.Key, Count = g.Count() })
    .OrderByDescending(g => g.Count)
    .First();



    //Bonus statistic of Average Messages per user per day!
    var averageMessagesPerUserPerDay = context.Users.Include(u => u.Messages)
    .Select(u => new
    {
        UserName = u.Username,
        MessageCount = u.Messages.Count,
        ActiveDays = u.Messages.Select(m => m.CreatedAt.Date).Distinct().Count()
    })
    .ToList();

    double totalAverage = averageMessagesPerUserPerDay
        .Select(u => (double)u.MessageCount / u.ActiveDays)
        .Average();



    Console.WriteLine();
    Console.WriteLine("Most common messages:");

    foreach (var message in mostCommonMessages)
    {
        Console.WriteLine($"Message: {message.Message}, Count: {message.Count}");
    }

    Console.WriteLine();
    Console.WriteLine("Most common messages by user:");
    foreach (var userMessage in mostCommonMessagesByUser)
    {
        Console.WriteLine($"User: {userMessage.UserName}, Message: {userMessage.MostCommonMessage.Message}, Count: {userMessage.MostCommonMessage.Count}");
    }

    Console.WriteLine();
    Console.WriteLine($"Hour with the most messages: {messagesByHour.Hour}:00 - {messagesByHour.Hour + 1}:00");

    Console.WriteLine();
    Console.WriteLine($"Average messages per user per day: {totalAverage}");

    Console.WriteLine();
    Console.WriteLine("Messages from the last session:");
    foreach (var u in users) // Try to have messages shown for existing users relogging in 
    {
        Console.WriteLine($"{u.Name} wrote {u.Messages.Count} messages.");
    }


}
