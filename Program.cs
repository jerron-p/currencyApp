using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class Program
{
static void Main(string[] args)
      {
         //Connect to the database
         SQLiteConnection conn = new SQLiteConnection("Data Source=database.db");

         //THE MAIN PROGRAM THAT THE USER WILL INTERACT WITH
        Console.WriteLine("Hello and welcome to the Currency Converter!\n");
         bool continueUsing = true;
         bool userInputValid = true;
         string userInput;
         List<Tuple<string, double>> allCurrencies;
         List<Currency> currencies;
         do
         {
            //Ask the user what they would like to do
            do{
                Console.WriteLine("Please enter a number corresponding to what you would like to do: ");
                Console.WriteLine("0 - Exit");
                Console.WriteLine("1 - Get the exchange rate between two currencies");
                Console.WriteLine("2 - Enter a new currency exchange rate into the database");
                Console.WriteLine("3 - View all currencies in the database and their exchange rate to USD");
                Console.WriteLine("4 - Update one of the currencies");
                Console.WriteLine("5 - Remove one of the currencies from the database");
                userInput = Console.ReadLine();
                if(userInput == "0" || userInput == "1" || userInput == "2" || userInput == "3" || userInput == "4" || userInput == "5")
                {
                    userInputValid = false;
                } 
                else
                {
                    Console.WriteLine("I'm sorry, but you entered \"{0}\" which is not a valid option at this time.\n\n", userInput);
                }
            } while(userInputValid);
            //Call up the appropriate method based on the user's input
            switch(userInput)
            {
                case "0":
                continueUsing = false;
                Console.WriteLine("\n\nThank you for using the Currency Converter, have a pleasant day!");
                break;

                case "1":
                //Get the currencies from the database
                allCurrencies = ReadData(conn);
                //Create a list of currency objects
                currencies = new List<Currency>();
                foreach(var item in allCurrencies)
                {
                    currencies.Add(new Currency(item.Item1, item.Item2));
                }
                ConvertCurrency(currencies);
                break;

                case "2":
                InsertData(conn);
                break;

                case "3":
                allCurrencies = ReadData(conn);
                currencies = new List<Currency>();
                foreach(var item in allCurrencies)
                {
                    currencies.Add(new Currency(item.Item1, item.Item2));
                }
                //Print out the list to the user
                int indexTracker = 0;
                Console.Write("\n\n");
                foreach(Currency currency in currencies)
                {
                    Console.WriteLine(currencies[indexTracker].CurrencyCode() + ": " + currencies[indexTracker].ExchangeRate());
                    indexTracker++;
                }
                Console.Write("\n");
                break;
                
                case "4":
                //Get the currencies from the database
                allCurrencies = ReadData(conn);
                //Create a list of currency objects
                currencies = new List<Currency>();
                foreach(var item in allCurrencies)
                {
                    currencies.Add(new Currency(item.Item1, item.Item2));
                }
                UpdateCurrency(conn, currencies);
                break;

                case "5":
                //Get the currencies from the database
                allCurrencies = ReadData(conn);
                //Create a list of currency objects
                currencies = new List<Currency>();
                foreach(var item in allCurrencies)
                {
                    currencies.Add(new Currency(item.Item1, item.Item2));
                }
                DeleteCurrency(conn, currencies);
                break;
            }
         } while (continueUsing);
      }

/* ------------------------ Methods ----------------------------------------------- */

    // DELETE A CURRENCY FROM THE DATABASE
    static void DeleteCurrency(SQLiteConnection conn, List<Currency> currencies)
    {
        bool userInputValid = true;
        string userInput;
        string confirmUserInput;
        string targetCurrencyCode;
        int indexTracker;
        List<string> currencyCheck = new List<string>();

        do
        {
            indexTracker = 0;
            Console.WriteLine("\nHere is the list of currencies currently in our database:");
            foreach(Currency currency in currencies)
            {
                Console.WriteLine(currencies[indexTracker].CurrencyCode());
                currencyCheck.Add(currencies[indexTracker].CurrencyCode());
                indexTracker++;
            }
            Console.WriteLine("\nPlease enter the three letter code corresponding to the currency that you want to delete.");
            userInput = Console.ReadLine();
            //Change it to upper case
            targetCurrencyCode = userInput.ToUpper();
            //Check that it is a valid currency code
            if(currencyCheck.Contains(targetCurrencyCode))
            {
                userInputValid = false;
            }
            else{
                Console.WriteLine("\nI'm sorry, but you entered \"{0}\" which is not a valid currency code.\n", userInput);
            }
        }while(userInputValid);
        userInputValid = true;
        //Double check
        do
        {
            Console.WriteLine("Are you sure that you want to remove {0} from the database? yes/no", targetCurrencyCode);
            userInput = Console.ReadLine();
            confirmUserInput = userInput.ToUpper();
            if(confirmUserInput == "YES" || confirmUserInput == "Y")
            {
                //Remove the currency from the database
                userInputValid = false;
                conn.Open();
                string statement = "DELETE FROM currencies WHERE currencyCode=\'" + targetCurrencyCode + "\';";
                SQLiteCommand cmd;
                cmd = conn.CreateCommand();
                cmd.CommandText = statement;
                cmd.ExecuteNonQuery();
                conn.Close();
                Console.WriteLine("Currency successfully deleted!\n");
            }
            else if(confirmUserInput == "NO" || confirmUserInput == "N")
            {
                userInputValid = false;
            }
            else{
                Console.WriteLine("\nI'm sorry but you enetered \"{0}\", please enter either \"yes\" or \"no\".\n", userInput);
            }
        }while(userInputValid);
    }

    //UPDATE EXISTING CURRENCY
    static void UpdateCurrency(SQLiteConnection conn, List<Currency> currencies)
    {
        bool userInputValid = true;
        string userInput;
        string targetCurrencyCode;
        int indexTracker;
        string newCurrencyCode;
        double newExchangeRate;
        List<string> currencyCheck = new List<string>();
        do
        {
            indexTracker = 0;
            Console.WriteLine("\nHere is the list of currencies currently in our database:");
            foreach(Currency currency in currencies)
            {
                Console.WriteLine(currencies[indexTracker].CurrencyCode());
                currencyCheck.Add(currencies[indexTracker].CurrencyCode());
                indexTracker++;
            }
            Console.WriteLine("\nPlease enter the three letter code corresponding to the currency that you want to update.");
            userInput = Console.ReadLine();
            //Change it to upper case
            targetCurrencyCode = userInput.ToUpper();
            //Check that it is a valid currency code
            if(currencyCheck.Contains(targetCurrencyCode))
            {
                userInputValid = false;
            }
            else{
                Console.WriteLine("\nI'm sorry, but you entered \"{0}\" which is not a valid currency code.\n", userInput);
            }
        }while(userInputValid);

        //Ask if the user is wanting to change the currency code, exchange rate, or both
        userInputValid = true;
        targetCurrencyCode = userInput.ToUpper();
        do
        {
            Console.WriteLine("\nPlease select one of the following options that you wish to do to {0}:", targetCurrencyCode);
            Console.WriteLine("0 - I wish to update the three letter currency code.");
            Console.WriteLine("1 - I wish to update the exchange rate.");
            userInput = Console.ReadLine();
            if(userInput == "0" || userInput == "1" || userInput == "2")
            {
                userInputValid = false;
            } 
            else
            {
                Console.WriteLine("I'm sorry, but you entered \"{0}\" which is not a valid option at this time.\n\n", userInput);
            }
        }while(userInputValid);
        switch(userInput)
        {
            case "0":
            //Update the three letter code
            userInputValid = true;
            do
                {
                Console.WriteLine("\nPlease enter the new three letter code: ");
                userInput = Console.ReadLine();
                //Change it to upper case
                newCurrencyCode = userInput.ToUpper();
                //Check that it is a valid currency code
                if(newCurrencyCode.Length == 3)
                {
                    userInputValid = false;
                    //Update the currency code
                    conn.Open();
                    string statement = "UPDATE currencies SET currencyCode = \'" + newCurrencyCode + "\' WHERE currencyCode = \'" + targetCurrencyCode + "\';";
                    SQLiteCommand cmd;
                    cmd = conn.CreateCommand();
                    cmd.CommandText = statement;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    Console.WriteLine("Currency successfully updated!\n");
                }
                else{
                    Console.WriteLine("\nI'm sorry, but you entered \"{0}\" which is not a valid currency code. Please ensure that the lenght is exactly three letters long.\n", userInput);
                }
            }while(userInputValid);
            break;

            case "1":
            //Update exchange rate
            userInputValid = true;
            do{
                Console.WriteLine("\nPlease enter the new exchange rate into USD: ");
                //Check that the user entered a number
                userInput = Console.ReadLine();
                bool isNumb = Double.TryParse(userInput, out newExchangeRate);
                if(isNumb)
                {
                    //Update the exchange rate
                    userInputValid = false;
                    conn.Open();
                    string statement = "UPDATE currencies SET exchangeRate = " + newExchangeRate + " WHERE currencyCode = \'" + targetCurrencyCode + "\';";
                    Console.WriteLine(statement);
                    SQLiteCommand cmd;
                    cmd = conn.CreateCommand();
                    cmd.CommandText = statement;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    Console.WriteLine("Currency successfully updated!\n");

                }
                else
                {
                    Console.WriteLine("\n\nI'm sorry, but \"{0}\" is not a valid number, please enter a new number.", userInput);
                }

            }while(userInputValid);
            break;
        }
    }

    //CONVERT BETWEEN TWO CURRENCIES
    static void ConvertCurrency(List<Currency> currencies)
    {
        //Print out the available currencies and ask the user to input the three character code
        bool userInputValid = true;
        string userInput;
        string convertFromCode;
        double convertToRate;
        string convertToCode;
        double convertFromRate;
        double convertAmount;
        double convertedCurrency;
        int indexTracker;
        List<string> currencyCheck = new List<string>();
        do
        {
            indexTracker = 0;
            Console.WriteLine("\nHere is the list of currencies currently in our database:");
            foreach(Currency currency in currencies)
            {
                Console.WriteLine(currencies[indexTracker].CurrencyCode());
                currencyCheck.Add(currencies[indexTracker].CurrencyCode());
                indexTracker++;
            }
            Console.WriteLine("\nPlease enter the three letter code corresponding to the currency that you want to convert from.");
            userInput = Console.ReadLine();
            //Change it to upper case
            convertFromCode = userInput.ToUpper();
            //Check that it is a valid currency code
            if(currencyCheck.Contains(convertFromCode))
            {
                userInputValid = false;
            }
            else{
                Console.WriteLine("\nI'm sorry, but you entered \"{0}\" which is not a valid currency code.\n", userInput);
            }
            
        }while(userInputValid);
        //Ask for what currency the user wants to convert to
        userInputValid = true;
        do
        {
            indexTracker = 0;
            Console.WriteLine("\n\nHere is the list of currencies currently in our database:");
            foreach(Currency currency in currencies)
            {
                Console.WriteLine(currencies[indexTracker].CurrencyCode());
                indexTracker++;
            }
            Console.WriteLine("\nPlease enter the three letter code corresponding to the currency that you want to convert to.");
            userInput = Console.ReadLine();
            //Change it to upper case
            convertToCode = userInput.ToUpper();
            //Check that it is a valid currency code
            if(currencyCheck.Contains(convertFromCode))
            {
                userInputValid = false;
            }
            else{
                Console.WriteLine("\n\nI'm sorry, but you entered \"{0}\" which is not a valid currency code.", userInput);
            }
            
        }while(userInputValid);

        //Ask the user how much money they want to convert
        userInputValid = true;
        do{
            Console.WriteLine("\nEnter the amount of {0} that you want to convert to {1}", convertFromCode, convertToCode);
            //Check that the user entered a number
            userInput = Console.ReadLine();
            bool isNumb = Double.TryParse(userInput, out convertAmount);
            if(isNumb)
            {
                //calculate the exchange amount (ToRate/FromRate) * ConvertAmount = ConvertedCurrency
                userInputValid = false;
                convertToRate = currencies[currencyCheck.IndexOf(convertToCode)].ExchangeRate();
                convertFromRate = currencies[currencyCheck.IndexOf(convertFromCode)].ExchangeRate();
                convertedCurrency = (convertToRate / convertFromRate) * convertAmount;
                //Print the conversion
                Console.WriteLine("{0} {1} converts to {2} {3}",convertAmount, convertFromCode, convertedCurrency, convertToCode);
            }
            else
            {
                Console.WriteLine("\n\nI'm sorry, but \"{0}\" is not a valid number, please enter a new number.", userInput);
            }

        }while(userInputValid);
    }




    //GRAB ALL CURRENCY CODES AND EXCHANGE RATES FROM THE DATABASE
      static List<Tuple<string, double>> ReadData(SQLiteConnection conn)
      {
        List<Tuple<string, double>> dbInfo = new List<Tuple<string, double>>();
        conn.Open();
        string statement = "SELECT currencyCode, exchangeRate FROM currencies";
        using var cmd = new SQLiteCommand(statement, conn);
        using SQLiteDataReader rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            dbInfo.Add(Tuple.Create(rdr.GetString(0), rdr.GetDouble(1)));
        }
        conn.Close();
        return dbInfo;
      }


      
//INSERT NEW CURRENCY INTO DATABASE
    static void InsertData(SQLiteConnection conn)
    {
        string currencyCode;
        string userInput;
        bool userInputValid = true;
        double exchangeRate;
        //Ask for the three letter code of the currency
        do{
            Console.WriteLine("\nPlease enter the three letter code of the currency that you wish to add: ");
            userInput = Console.ReadLine();
            //Verify that the code is three letters
            if(userInput.Length != 3)
            {
                Console.WriteLine("\nI'm sorry, but you enetered \"{0}\", which is not three letters long.", userInput);
            }
            else{
                userInputValid = false;
            }
        }while(userInputValid);
        currencyCode = userInput.ToUpper();
        //Ask for the exchange rate to USD
        userInputValid = true;
        do{
            Console.WriteLine("\nPlease enter the conversion rate from {0} to USD: ",currencyCode);
            userInput = Console.ReadLine();
            bool isNumb = Double.TryParse(userInput, out exchangeRate);
            if(isNumb)
            {
                userInputValid = false;
                //Connect to the database and add the new currency
                conn.Open();
                string statement = "INSERT INTO currencies (currencyCode, exchangeRate) VALUES (\'" + currencyCode + "\', " + exchangeRate.ToString() + ");";
                SQLiteCommand cmd;
                cmd = conn.CreateCommand();
                cmd.CommandText = statement;
                cmd.ExecuteNonQuery();
                conn.Close();
                Console.WriteLine("Currency successfully added!\n");
            }
            else{
                Console.WriteLine("\n\nI'm sorry, but \"{0}\" is not a valid number, please enter a new number.", userInput);
            }

        }while(userInputValid);
    }

/*-------------------------------End of Methods---------------------------------------------*/
}


//Create the class for the currencies
class Currency
{
    string currencyCode;
    double exchangeRate;
    public Currency(string code, double rate)
    {
        currencyCode = code;
        exchangeRate = rate;
    }

    public string CurrencyCode()
    {
        return currencyCode;
    }
    public double ExchangeRate()
    {
        return exchangeRate;
    }
}
