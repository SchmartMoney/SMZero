# SMZero

Functional Requirements 
Main Map 
• Description: 
o The main map serves as the central starting point of the application. 
o There are 8 zones (displayed by main buildings), of which 7 have no functionality 
and are purely decorative. 
o One zone (Vault Avenue) is clickable and offers interaction options. 
1. Clickable Zone 
• Interaction: 
o Clicking on the zone opens a text box with: 
▪ 3 options: 
1. "Enter Zone": Leads the user into the next zone. 
2. "Close/Leave": Closes the text box and returns to the main map. 
3. "x" (Close Button): Closes the text box directly. 
▪ A short explanatory text about the zone, describing its functionality (in a few 
sentences). 
2. Text Box Behavior 
• Opening: 
o The text box appears over the map, slightly overlapping the building (modal window). 
• Closing: 
o Clicking "Close/Leave" or the "x": 
▪ Closes the text box and returns the user to the main map. 
▪ No additional effects are required. 
• Action "Enter Zone": 
o The text box disappears. 
o A zoom-in effect is performed, followed by a blur or fade-to-black effect. 
o If loading takes longer: 
▪ A loading screen is displayed. 
o After loading, the next zone is displayed. 
3. Second Layer (After "Enter Zone") 
• Loading Effects: 
o Upon entering the second zone: 
▪ A small fade-out effect is performed. 
▪ The user is then shown the full zone layer. 
• Display: 
o The second zone is displayed completely visible, with no zoom or other limitations. 
Functional Requirements: Zone Functionality 
1. Zone Overview 
• Buildings: 
o The zone contains 10 buildings. For the initial functionality, only one building can be 
active and interactable. 
o The other buildings remain decorative and non-functional but can be activated in 
future updates (or if we have more time to make them functional). 
• GUI Slots: 
o At the top of the zone interface, there are 10 zone slots displayed in the GUI. 
▪ First Slot: Active by default and can be populated with a Zone NFT. 
▪ Remaining Slots: Initially grayed out and inactive. They will become active as 
per game progression or future functionality. 
2. Zone NFT Staking 
• Staking Mechanism (called staking but it has nothing to do with blockchain yet, ingame 
assets are pseudo NFTs as previously mentioned: 
o The first active zone slot can accept a Zone NFT for staking. 
o Once a Zone NFT is staked: 
▪ The associated building becomes active and clickable. 
3. Active Building Interaction 
• Click Behavior: 
o Clicking on the active building opens a popup window with the following 
components: 
▪ Description: Brief explanation of the building’s functionality and the assets it 
produces. 
▪ 3D Building Model: A rendered 3D graphic of the building for visual appeal. 
▪ Timer: Indicates the time remaining for asset production. 
▪ Character NFT Slots: Three slots for placing Character NFTs. 
4. Character NFT Staking 
• Initial State: 
o Without any Character NFTs placed in the slots, the building remains inactive, and no 
processes occur. 
• When a Character NFT is Staked: 
o The timer starts running to indicate the production process. 
o Additional Character NFTs can be staked in the remaining slots to potentially enhance 
production (basic features – e.g. character X increases production speed, character Y 
increases produced amount, character Z imporves both). 
5. Timer Completion 
• Upon Timer Expiry: 
o The timer transitions to a "Collect Asset" button. 
• When "Collect Asset" is Clicked: 
o The player receives a specific amount of Fortune Dollars credited to their account. 
o The timer automatically resets and starts the production process again. 
Marketplace 
1. Marketplace Overview 
• Location: 
o The "Twilight Square" on the main map will serve as the initial marketplace. 
• Interaction: 
o Clicking on the "Twilight Square" opens a popup window with the following features: 
▪ Options to purchase: 
▪ 3 different Character NFTs 
▪ Zone NFTs 
▪ Each asset is displayed with a "Buy Now" button beneath it. 
2. Purchase Process 
The purchase process will follow these steps, regardless of the variant: 
1. Open Marketplace Popup: 
o The player clicks on the "Twilight Square." 
o A popup window appears, showing the available assets. 
2. Select an Asset to Buy: 
o The player clicks the "Buy Now" button under the desired asset. 
o A confirmation popup appears, asking, "Are you sure you want to buy this asset?" 
▪ Options: Yes/No 
3. Confirmation Logic: 
o If the player selects "Yes": 
▪ Fortune Dollars (player currency) are deducted from their account. 
▪ If the player has insufficient funds: 
▪ An error message appears: "You do not have enough Fortune 
Dollars to complete this purchase." 
▪ If the purchase is successful: 
▪ The asset is either added to the Player Inventory (Variant A) or 
directly placed in an available slot in the corresponding zone (Variant 
B). 
3. Variants for Asset Handling 
• Variant A: Player Inventory 
o Description: 
▪ Purchased assets (Character NFTs or Zone NFTs) are stored in a Player 
Inventory. 
▪ The player can navigate to the appropriate zone/building, select an open slot, 
and stake the asset manually. 
o Process: 
▪ After purchase, the asset appears in the Player Inventory. 
▪ The player visits the relevant zone/building and clicks on an open slot to 
assign the asset. 
▪ Once staked, the slot becomes active (Character or Zone NFT functionality 
begins). 
• Variant B: Automatic Slot Placement 
o Description: 
▪ Purchased assets are automatically placed in the first available slot in the 
relevant zone/building. 
▪ No Player Inventory is required. 
o Process: 
▪ After purchase, the system automatically assigns the asset to the appropriate 
slot. 
▪ If no slot is available, an error message appears: "No available slots for this 
asset." 
▪ The asset immediately becomes active once placed. 