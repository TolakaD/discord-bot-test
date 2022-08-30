using System.Collections.Generic;
using System.Linq;
using DiscordBotTest.Commands;

namespace DiscordBotTest.Model
{
    public class Researches
    {
        public List<Research> Items = new List<Research>
        {
            new Research("World Gate", "**World gate**\n Guthix brought us to Gielinor through the World Gate."),
            new Research("Second Research", "**Second Research**\n This is some random text for a second research."),
            new Research("Third Research", "**Third Research**\n This is some random text for a third research."),
        };

        public Researches()
        {
            SelectedIndex = 0;
        }

        public int SelectedIndex { get; set; }
        public string CurrentDescription => Items[SelectedIndex].Description;

        public void SelectDown()
        {
            if (SelectedIndex == Items.Count - 1)
            {
                SelectedIndex = 0;
                return;
            }

            SelectedIndex++;
        }

        public void SelectUp()
        {
            if (SelectedIndex == 0)
            {
                SelectedIndex = Items.Count - 1;
                return;
            }

            SelectedIndex--;
        }

        public void SelectResearch(string title)
        {
            SelectedIndex = Items.FindIndex(i => i.Name.Equals(title));
        }
    }
}