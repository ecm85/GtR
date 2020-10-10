# GtR Printing Guide
This guide will attempt to walk through the steps needed to generate files for Glory to Rome.

* Step 1: Decide what site you want to print from and what card size you want. This will inform your decisions later for configuration.
  * For example, if you want standard size cards (Poker card sized, used for MTG and tons of other games) you will want the images to be 2.5" by 3.5".
  * For the printer, PrintPlayGames (https://www.printplaygames.com/) takes the cards in sheets of 18 cards, where other sites like Gamecrafter take them as individual cards.
* Step 2: Download the latest release of the app.
* Step 3: Install the fonts included in the release.
  * NeuzeitGro-RegModified.ttf
  * NeuzeitGro-Bol.ttf
* Step 4: Open GtrConfig.json and change the values as needed.
  * You can change the following values:
    * cardShortSideInInches - This is the smaller dimension of the cards in inches. Default value (already in app.config): 2.5
    * cardLongSideInInches - This is larger dimension of the cards, in inches. Default value (already in app.config): 3.5
    * bleedSizeInInches - This is the amount of bleed to include outside the bounds of the card image. Default value (already in app.config): .125
    * borderPaddingInInches - This is the extra padding -inside the valid printing area- to leave blank. Default value (already in app.config): .0625
    * saveConfiguration - This is the output type, can be SingleImage or Page. Default value (already in app.config): Page
* Step 5: Run the app.
  * Windows: Run Gtr.exe. 
    * Note for power users: You can run this via double-click in Windows and it will prompt for input when complete; if run via a CLI it will not prompt.
  * OSX:
    * chmod a+x GtR
    * Run GtR
* Step 6: Open the image directory (the app tells you where it put them).
* Step 7: Upload the images to the site of your choice, pay them, and wait for GtR to arrive on your doorstep!
  * The various printers will usually require you to give them a front image and a back image for each card to be printed.
  * If you're doing sheets, you will need
    * 3 copies of each order card sheet, with the order card back for each (9 sheets total).
    * 2 copies of the site cards sheet, with the site cards back for both (2 sheets total).
    * 1 copy of the misc sheet, with the misc sheet back (has the leader card (double sided), the jacks (one side sword, other side quill) and the merchant bonus cards (double sided).
  * If you're doing individual cards, you will need
    * 3 copies of each order card, with the order card back for each.
      * Note that you will need 6 copies of each Wood/Craftsman/Green card and each Rubble/Laborer/Yellow card, which is why the app makes two of each of those.
    * 36 total sites, 6 of each material, with the site back.
    * 1 double-sided Leader
    * 6 Jacks, with sword on one side and quill on the other
    * 6 total double-sided Merchant bonuses, 1 of each material
* Step 7b: Decide on the material and finish for your cards
  * There's no right or wrong answer here.
  * I went for PrintPlayGames's cheapest option, since we sleeve everything anyway (even the Trash card in Dominion!).
  * You can spend more for a nicer finish and/or plastic cards if you like.
* Step 8: Figure out an option for the player boards.
  * For my copy, all I did was print the ones available at https://www.dropbox.com/sh/z5psnbb8ypmbxpz/AABSNRKSbVs2R_fYBBQExSEHa/Public%20(1)?dl=0&preview=GtR-big+player+board.png&subfolder_nav_tracking=1
  (Full credit to Arachnode from BGG for that file)
  * I got them printed at FedEx for about $5 onto 100lb glossy cardstock.
  * This only gives 1-sided boards. Personally I felt that the other side (with the card flow) was really dense and unhelpful. If you want it you'll need to find it yourself.
* Step 9: Figure out what you want to do for the following other components:
  * Box: You can put a lot of effort into a great box. I didn't and just used a box from Stonemaier Realistic Resources I had lying around.
  * Rome Demands mat: This seemed even more useless to me than the reverse side of the player boards, so I ignored it.
  * Rules: They're available at https://cdn.1j1ju.com/medias/e2/0b/89-glory-to-rome-rulebook.pdf. I didn't bother printing them, but you can probably just do regular paper or something fancy if you feel up to it.
  * Small Player Mat cards: These seemed like the least useful component in the box, so I skipped them entirely.

Good luck and Glory to Rome!

--Storm


"If I have seen further it is by standing on the shoulders of Giants."
My thanks to:
  * Carl Chudyk for the amazing game
  * Arachnode on BGG for doing a ton of work on his copy, his images were invaluable for this production
  * My wife for letting me spend hours just tinkering with a card generator for no reason (and for beating me with a Catacomb-ending on her first play).



Future updates:
* Promo cards
* Supporting Macs? (Involves converting to .net Core and retesting)