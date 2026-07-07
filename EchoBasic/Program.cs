using EchoBasic;

var running = true;

while (running)
{
    Console.WriteLine();
    Console.Write("> ");

    var input = Console.ReadLine();
    input = input.ToUpper();
    if (!string.IsNullOrEmpty(input))
    {
        switch (input)
        {
            case "EXIT":
                running = false;
                break;
            case "RUN":
                try
                {
                    Runtime.Run();
                }
                catch (BasicException ex)
                {
                    Console.WriteLine($"Error at line {ex.LineNumber}: {ex.Message}");
                }
                break;
            case "LIST":
                Storage.ListProgram();
                break;
            case "NEW":
                Storage.Clear();
                break;
            default:
            {
                var tokens = Parser.Tokenise(input, true);
                if (!tokens.Any())
                {
                    continue;
                }

                if (tokens[0].Type == TokenType.LineNumber)
                {
                    try
                    {
                        var number = ((LineNumberToken)tokens[0]).LineNumber;
                        var numberString = number.ToString();
                        var lineText = input.Substring(numberString.Length).TrimStart();
                        Storage.AddLine(number, lineText);
                    }
                    catch(BasicException ex)
                    {
                        Console.WriteLine($"Error at line: {ex.LineNumber} -> {ex.Message}");
                    }
                }
                else
                {
                    try
                    {
                        Runtime.RunLine(tokens);
                    }
                    catch (BasicException ex)
                    {
                        Console.WriteLine($"Error -> {ex.Message}");
                    }
                }

                break;
            }
        }
    }
}
