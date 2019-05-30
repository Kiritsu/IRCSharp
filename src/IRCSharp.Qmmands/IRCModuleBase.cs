using Qmmands;

namespace IRCSharp.Qmmands
{
    public sealed class IRCModuleBase : ModuleBase<IRCCommandContext>
    {
        public void Respond(string content)
        {
            if (Context.Channel != null)
            {
                Context.Channel.SendMessage(content);
            }
            else
            {
                Context.Author.SendMessage(content);
            }
        }
    }
}
