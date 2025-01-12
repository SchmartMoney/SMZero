mergeInto(LibraryManager.library, {
    IsPortraitOrientation: function() {
        // Simple check based on dimensions
        var isPortrait = window.innerHeight > window.innerWidth;
        console.log('Current dimensions:', window.innerWidth, 'x', window.innerHeight, '=', isPortrait ? 'Portrait' : 'Landscape');
        return isPortrait;
    },

    SubscribeToOrientationChange: function() {
        function checkOrientation() {
            // Add a small delay to ensure dimensions have updated
            setTimeout(function() {
                try {
                    var unityInstance = window.gameInstance;
                    if (unityInstance) {
                        unityInstance.SendMessage("RotationOverlay", "CheckOrientation");
                        console.log("Orientation check sent to Unity");
                    } else {
                        console.error("Unity instance not found");
                    }
                } catch (error) {
                    console.error("Error in checkOrientation:", error);
                }
            }, 200);
        }

        window.addEventListener('orientationchange', function() {
            console.log('Orientation change event fired');
            checkOrientation();
        });
        
        window.addEventListener('resize', function() {
            console.log('Resize event fired');
            checkOrientation();
        });

        // Initial check
        checkOrientation();
    },

    InitTelegramWebApp: function() {
        try {
            console.log("Initializing Telegram WebApp...");
            if (!window.Telegram || !window.Telegram.WebApp) {
                console.error("Telegram WebApp not found. Make sure the script is loaded.");
                return false;
            }

            // Initialize and expand to full height
            window.Telegram.WebApp.ready();
            window.Telegram.WebApp.expand();

            console.log("Telegram WebApp initialized successfully");
            console.log("Init Data:", window.Telegram.WebApp.initData);
            console.log("User:", window.Telegram.WebApp.initDataUnsafe.user);
            
            return true;
        } catch (error) {
            console.error("Failed to initialize Telegram WebApp:", error);
            return false;
        }
    },

    GetTelegramUserId: function() {
        try {
            console.log("Getting Telegram User ID...");
            if (!window.Telegram || !window.Telegram.WebApp) {
                console.error("Telegram WebApp not available");
                return 0;
            }

            var user = window.Telegram.WebApp.initDataUnsafe.user;
            console.log("User data:", user);

            if (user && user.id) {
                var idStr = user.id.toString();
                console.log("User ID:", idStr);
                var bufferSize = lengthBytesUTF8(idStr) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(idStr, buffer, bufferSize);
                return buffer;
            }
            console.error("User ID not found in WebApp data");
            return 0;
        } catch (error) {
            console.error("Failed to get Telegram user ID:", error);
            return 0;
        }
    },

    IsTelegramWebApp: function() {
        try {
            console.log("Checking if running in Telegram WebApp...");
            var isTelegram = !!(window.Telegram && window.Telegram.WebApp);
            console.log("Is Telegram WebApp:", isTelegram);
            return isTelegram;
        } catch (error) {
            console.error("Error checking Telegram WebApp:", error);
            return false;
        }
    }
});