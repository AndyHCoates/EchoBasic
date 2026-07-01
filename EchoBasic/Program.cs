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
                Runtime.Run();
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
                    var number = ((LineNumberToken)tokens[0]).LineNumber;
                    var numberString = number.ToString();
                    var lineText = input.Substring(numberString.Length).TrimStart();
                    Storage.AddLine(number, lineText);
                }
                else
                {
                    Runtime.RunLine(tokens);
                }

                break;
            }
        }
    }
}
