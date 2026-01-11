using System.Collections.Generic;
using System.Linq;

namespace Vintage.NewBootOptions.Features;

public sealed class ASpecificCardOffering : CardAction
{
	public List<Card> Cards { get; set; } = [];
	public bool CanSkip { get; set; } = false;
	public CardDestination Destination { get; set; } = CardDestination.Deck;

	public override Route? BeginWithRoute(G g, State s, Combat c)
	{
		timer = 0;

		return new CardReward
		{
            cards = Cards.Select(c =>
            {
                c.drawAnim = 1;
                c.flipAnim = 1;
                return c;
            }).ToList(),
            canSkip = CanSkip
        };

		/*return ModEntry.Instance.KokoroApi.CardDestination.ModifyCardReward(
			new CardReward
			{
				cards = Cards.Select(c =>
				{
					c.drawAnim = 1;
					c.flipAnim = 1;
					return c;
				}).ToList(),
				canSkip = CanSkip
			}
		).SetDestination(Destination).AsRoute;*/
	}

	public override List<Tooltip> GetTooltips(State s)
		=> [
			new TTGlossary("action.cardOffering")
		];
}
