import React, { Component } from 'react';

export class Instructions extends Component {
    static displayName = Instructions.name;

    render() {
        return (
            <>
                <div className='card mb-3'>
                    <div className='card-header'>
                         Glory to Rome Printing Guide
                    </div>
                    <div className='card-body'>
                        <div className='card-text'>This guide will attempt to walk through the steps needed to generate files for Glory to Rome.</div>
                    </div>
                </div>
                <div className='card mb-3'>
                    <div className='card-header'>
                        Step 1: Printing Site
                    </div>
                    <div className='card-body'>
                        <div className='card-text'>
                            Decide what site you want to print from and what card size you want. This will inform your decisions later for configuration.
                            <ul>
                                <li>For example, if you want standard size cards (Poker card sized, used for MTG and tons of other games) you will want the images to be 2.5" by 3.5".</li>
                                <li>For the printer, PrintPlayGames (https://www.printplaygames.com/) takes the cards in sheets of 18 cards, where other sites like Gamecrafter take them as individual cards.</li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div className='card mb-3'>
                    <div className='card-header'>
                        Step 2: Create Images
                    </div>
                    <div className='card-body'>
                        <div className='card-text'>
                            On the main page of this site, input your values and generate the images. You can change:
                            <ul>
                                <li>Card short side, in inches - This is the smaller dimension of the cards in inches. Default value: 2.5</li>
                                <li>Card long side, in inches - This is larger dimension of the cards, in inches. Default value: 3.5</li>
                                <li>Bleed size, in inches - This is the amount of bleed to include outside the bounds of the card image. Default value: .125</li>
                                <li>Border padding, in inches (default is 1/16th of an inch) - This is the extra padding -inside the valid printing area- to leave blank. Default value: .0625</li>
                                <li>Cards To Include - You can choose which cards you need. Default value: Everything but promo cards.</li>
                                <li>Card Configuration - This is the output type, can be 'Pages of Cards' or 'One Image Per Card'. Default value: 'Pages of Cards'</li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div className='card mb-3'>
                    <div className='card-header'>
                        Step 3: Upload Images
                    </div>
                    <div className='card-body'>
                        <div className='card-text'>
                            Upload the images to the site of your choice, pay them, and wait for GtR to arrive on your doorstep!
                            <ul>
                                <li>
                                    The various printers will usually require you to give them a front image and a back image for each card to be printed.
                                    <ul>
                                        <li>
                                            If you're doing sheets, you will need
                                            <ul>
                                                <li>3 copies of each order card sheet, with the order card back for each (9 sheets total).</li>
                                                <li>2 copies of the site cards sheet, with the site cards back for both (2 sheets total).</li>
                                                <li>1 copy of the misc sheet, with the misc sheet back (has the leader card (double sided), the jacks (one side sword, other side quill) and the merchant bonus cards (double sided).</li>
                                            </ul>
                                        </li>
                                        <li>
                                            If you're doing individual cards, you will need
                                            <ul>
                                                <li>
                                                    3 copies of each order card, with the order card back for each.
                                                    <ul><li>Note that you will need 6 copies of each Wood/Craftsman/Green card and each Rubble/Laborer/Yellow card, which is why the app makes two of each of those.</li></ul>
                                                </li>
                                                <li>36 total sites, 6 of each material, with the site back.</li>
                                                <li>1 double-sided Leader</li>
                                                <li>6 Jacks, with sword on one side and quill on the other</li>
                                                <li>6 total double-sided Merchant bonuses, 1 of each material</li>
                                            </ul>
                                        </li>
                                    </ul>
                                </li>
                                <li>
                                    You will also need to pick a material and/or finish for your cards.
                                    <ul>
                                        <li>There's no right or wrong answer here.</li>
                                        <li>I went for PrintPlayGames's cheapest option, since we sleeve everything anyway (even the Trash card in Dominion!).</li>
                                        <li>You can spend more for a nicer finish and/or plastic cards if you like.</li>
                                    </ul>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div className='card mb-3'>
                    <div className='card-header'>
                        Step 4: Player Boards
                    </div>
                    <div className='card-body'>
                        <div className='card-text'>
                            You will need to figure out an option for the player boards.
                            <ul>
                                <li>
                                    For my copy, all I did was print the ones available on BGG: <a href='https://www.dropbox.com/sh/z5psnbb8ypmbxpz/AABSNRKSbVs2R_fYBBQExSEHa/Public%20(1)?dl=0&preview=GtR-big+player+board.png&subfolder_nav_tracking=1'>Link</a> (Full credit to Arachnode from BGG for that file)
                                    <ul>
                                        <li>I got them printed at FedEx for about $5 onto 100lb glossy cardstock.</li>
                                        <li>This only gives 1-sided boards. Personally I felt that the other side (with the card flow) was really dense and unhelpful. If you want it you'll need to find it yourself.</li>
                                    </ul>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div className='card mb-3'>
                    <div className='card-header'>
                        Step 5: Other Components
                    </div>
                    <div className='card-body'>
                        <div className='card-text'>
                            You will need to figure out what you want to do for the other components.
                            <ul>
                                <li>Box: You can put a lot of effort into a great box. I didn't and just used a box from Stonemaier Realistic Resources I had lying around.</li>
                                <li>Rome Demands mat: This seemed even more useless to me than the reverse side of the player boards, so I ignored it.</li>
                                <li>Rules: They're available online: <a href='https://cdn.1j1ju.com/medias/e2/0b/89-glory-to-rome-rulebook.pdf'>Link</a>. I didn't bother printing them, but you can probably just do regular paper or something fancy if you feel up to it.</li>
                                <li>Small Player Mat cards: These seemed like the least useful component in the box, so I skipped them entirely.</li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div className='card mb-3'>
                    <div className='card-header'>
                        Step 6: Play!
                    </div>
                    <div className='card-body'>
                        <div className='card-text'>
                            Good luck and Glory to Rome!
                        </div>
                    </div>
                </div>
                <div className='card mb-3'>
                    <div className='card-header'>
                        Acknowledgements
                    </div>
                    <div className='card-body'>
                        <div className='card-text'>
                            "If I have seen further it is by standing on the shoulders of Giants."
                             <div>My thanks to:</div>
                             <li>Carl Chudyk for the amazing game</li>
                             <li>Arachnode on BGG for doing a ton of work on his copy, his images were invaluable for this production</li>
                            <li>My wife for letting me spend hours just tinkering with a card generator for no reason (and for beating me with a Catacomb-ending on her first play).</li>
                            --Storm
                        </div>
                    </div>
                </div>
                <div className='card mb-3'>
                    <div className='card-header'>
                        Future
                    </div>
                    <div className='card-body'>
                        <div className='card-text'>
                            Some future updates (that might or might not happen)
                            <li>Page size configurable (images per page / size of page)</li>
                            <li>
                                Allow only printing individual cards. So far, the following cards have been fixed since inception:
                                <ul>
                                    <li>Domus Aurea></li>
                                    <li>Palace</li>
                                </ul>
                            </li>
                        </div>
                    </div>
                </div>
            </>
        );
    }
}
