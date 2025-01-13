mergeInto(LibraryManager.library, {

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
    },

    GetTelegramUsername: function() {
        try {
            var user = window.Telegram.WebApp.initDataUnsafe.user;
            if (user && user.username) {
                var str = user.username.toString();
                var bufferSize = lengthBytesUTF8(str) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(str, buffer, bufferSize);
                return buffer;
            }
            return 0;
        } catch (error) {
            console.error("Error getting username:", error);
            return 0;
        }
    },

    GetTelegramFirstName: function() {
        try {
            var user = window.Telegram.WebApp.initDataUnsafe.user;
            if (user && user.first_name) {
                var str = user.first_name.toString();
                var bufferSize = lengthBytesUTF8(str) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(str, buffer, bufferSize);
                return buffer;
            }
            return 0;
        } catch (error) {
            console.error("Error getting first name:", error);
            return 0;
        }
    },

    GetTelegramLastName: function() {
        try {
            var user = window.Telegram.WebApp.initDataUnsafe.user;
            if (user && user.last_name) {
                var str = user.last_name.toString();
                var bufferSize = lengthBytesUTF8(str) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(str, buffer, bufferSize);
                return buffer;
            }
            return 0;
        } catch (error) {
            console.error("Error getting last name:", error);
            return 0;
        }
    },

    GetTelegramLanguage: function() {
        try {
            var user = window.Telegram.WebApp.initDataUnsafe.user;
            if (user && user.language_code) {
                var str = user.language_code.toString();
                var bufferSize = lengthBytesUTF8(str) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(str, buffer, bufferSize);
                return buffer;
            }
            return 0;
        } catch (error) {
            console.error("Error getting language:", error);
            return 0;
        }
    },

    IsTelegramPremium: function() {
        try {
            var user = window.Telegram.WebApp.initDataUnsafe.user;
            return user && user.is_premium ? 1 : 0;
        } catch (error) {
            console.error("Error checking premium status:", error);
            return 0;
        }
    }
});