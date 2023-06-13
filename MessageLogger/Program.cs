using MessageLogger.Data;
using Microsoft.EntityFrameworkCore;

using (var context = new MessageLoggerContext())

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
    foreach (var u in users)
    {
        Console.WriteLine($"{u.Name} wrote {u.Messages.Count} messages.");
    }


}
