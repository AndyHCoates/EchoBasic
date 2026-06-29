using EchoBasic;

var running = true;
var calc = new Calculator();

while (running)
{
    Console.WriteLine();
    Console.Write("> ");

    var input = Console.ReadLine();
    if (input == "exit")
    {
        running = false;
    }
    else if (!string.IsNullOrEmpty(input))
    {
        var result = calc.Calculate(input);
        Console.WriteLine("Result: " + result);
    }
}
