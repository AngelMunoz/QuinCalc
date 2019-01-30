using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using QuinCalc.Models;
using Windows.UI;
using Windows.UI.StartScreen;

namespace QuinCalc.Services
{
  public class TileService
  {


    public async Task<(SecondaryTile, TileContent)> GenerateExpenseTile(Expense expense)
    {
      SecondaryTile tile = new SecondaryTile
      {
        TileId = $"Expense:{expense.Id}",
        DisplayName = "Quincalc",
        Arguments = $"expense={expense.Id}",
        Logo = new Uri("ms-appx:///Assets/Square44x44Logo.altform-unplated_targetsize-256.png")
      };
      await tile.RequestCreateAsync();

      tile.VisualElements.BackgroundColor = Colors.Lavender;
      TileContent content = GenerateTileContent(expense);
      return (tile, content);
    }


    private TileContent GenerateTileContent(Expense expense)
    {
      return new TileContent()
      {
        Visual = new TileVisual()
        {
          TileMedium = GenerateTileBindingMedium(expense),
        }
      };
    }

    private TileBinding GenerateTileBindingMedium(Expense expense)
    {
      return new TileBinding()
      {
        Content = new TileBindingContentAdaptive()
        {
          PeekImage = new TilePeekImage()
          {
            Source = new Uri("ms-appx:///Assets/Square44x44Logo.altform-unplated_targetsize-256.png").ToString(),
            HintCrop = TilePeekImageCrop.Circle
          },
          TextStacking = TileTextStacking.Center,

          Children =
            {
                new AdaptiveText()
                {
                    Text = $"{expense.Name},",
                    HintAlign = AdaptiveTextAlign.Center,
                    HintStyle = AdaptiveTextStyle.Base
                },

                new AdaptiveText()
                {
                    Text = $"Amount: {expense.Amount.ToString("C", CultureInfo.CurrentCulture)}",
                    HintAlign = AdaptiveTextAlign.Center,
                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                }
            }
        }
      };

    }
  }
}
