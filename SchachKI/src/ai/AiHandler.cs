using ChatGPT.Net;
using ChatGPT.Net.DTO.ChatGPT;
using SchachKI.src.game;
using System.Text;

namespace SchachKI.src.ai
{
    public class AiHandler
    {
        //TODO api key sicherer speichern
        private const string API_KEY = "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";//"AIzaSyCJbnukFLAdYgRDoEXwuevuRwjgsksUe1E";
        private string _conversationId;
        private ChatGpt _bot;
        private Difficulty _difficulty;
        private string _aiColor;
        private string _prompt;

        public AiHandler(Difficulty difficulty, string aiColor)
        {
            _aiColor = aiColor;
            _difficulty = difficulty;
            _prompt = Properties.Resources.prompt;
            _conversationId = DateTime.Now.ToString();
            ChatGptOptions options = new ChatGptOptions();
            options.Model = "gpt-4.1-mini-2025-04-14";
            _bot = new ChatGpt(API_KEY, options);
            _bot.SetConversationSystemMessage(_conversationId, Properties.Resources.instruction.Replace("{color}", aiColor).Replace("{difficulty}", _difficulty.ToString()));
        }

        private async Task<ChessMove> GetNextMove(string movesCode, string setup, string legalMoves)
        {
            string response = await _bot.Ask(getPrompt(movesCode, setup, legalMoves), _conversationId);
            Console.WriteLine("Ai response:");
            Console.WriteLine(response);
            if (response == null || response.Trim().Length != 4) return null;
            ChessMove move = new ChessMove(response);
            move.Color = _aiColor;
            Console.WriteLine("code: " + move.getCode());
            return move;
        }

        public async Task<ChessMove> getNextLegalMove(Game game, string movesCode, string setup, string legalMoves)
        {
            int attempts = 0;
            while (attempts < 3) 
            {
                ChessMove move = await GetNextMove(movesCode, setup, legalMoves);
                if (move != null && game.getRules().isMoveOkay(move.From + move.To, _aiColor)) return move;
                Console.WriteLine("KI wollte einen illegalen Zug machen, nochmal versuchen... (" + (move != null ? move.getCode() : ")"));
                attempts++;
            }
            Console.WriteLine("Ki konnte keinen legalen Zug finden.");
            return null;
        }

        private string getPrompt(string movesCode, string setup, string legalMoves)
        {
            return _prompt.Replace("{color}", _aiColor).Replace("{difficulty}", _difficulty.ToString()).Replace("{moves}", movesCode).Replace("{setup}", setup).Replace("{legal_moves}", legalMoves);
        }

    }
}
