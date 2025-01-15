mergeInto(LibraryManager.library, {
    SaveToCloud: function(key, data) {
        // Convert C# string parameters to JS strings
        var keyStr = UTF8ToString(key);
        var dataStr = UTF8ToString(data);
        
        console.log('=== Saving to Cloud Storage ===');
        console.log('Key:', keyStr);
        console.log('Data:', dataStr);
        
        // Check if Telegram WebApp is available
        if (window.Telegram && window.Telegram.WebApp) {
            try {
                console.log('Using Telegram Cloud Storage API');
                // Use Telegram Mini Apps cloud storage
                window.Telegram.WebApp.CloudStorage.setItem(keyStr, dataStr)
                    .then(() => {
                        console.log('=== Save Successful ===');
                        console.log('- Storage Type: Telegram Cloud');
                        console.log('- Key:', keyStr);
                        console.log('- Data Length:', dataStr.length);
                        console.log('=====================');
                    })
                    .catch((error) => {
                        console.error('=== Save Failed ===');
                        console.error('- Storage Type: Telegram Cloud');
                        console.error('- Key:', keyStr);
                        console.error('- Error:', error);
                        console.error('=================');
                    });
            } catch (error) {
                console.error('=== Error Accessing Telegram Storage ===');
                console.error('- Error:', error);
                console.error('=====================================');
            }
        } else {
            console.warn('=== Telegram WebApp Not Available ===');
            console.warn('Falling back to localStorage');
            try {
                localStorage.setItem(keyStr, dataStr);
                console.log('=== Save Successful ===');
                console.log('- Storage Type: Local Storage');
                console.log('- Key:', keyStr);
                console.log('- Data Length:', dataStr.length);
                console.log('=====================');
            } catch (error) {
                console.error('=== Save Failed ===');
                console.error('- Storage Type: Local Storage');
                console.error('- Key:', keyStr);
                console.error('- Error:', error);
                console.error('=================');
            }
        }
    },

    LoadFromCloud: function(key) {
        var keyStr = UTF8ToString(key);
        var data = null;
        
        console.log('=== Loading from Cloud Storage ===');
        console.log('Key:', keyStr);
        
        // Check if Telegram WebApp is available
        if (window.Telegram && window.Telegram.WebApp) {
            try {
                console.log('Using Telegram Cloud Storage API');
                // Use Telegram Mini Apps cloud storage
                window.Telegram.WebApp.CloudStorage.getItem(keyStr)
                    .then((value) => {
                        if (value) {
                            data = value;
                            console.log('=== Load Successful ===');
                            console.log('- Storage Type: Telegram Cloud');
                            console.log('- Key:', keyStr);
                            console.log('- Data:', value);
                            console.log('- Data Length:', value.length);
                            console.log('=====================');
                        } else {
                            console.log('=== No Data Found ===');
                            console.log('- Storage Type: Telegram Cloud');
                            console.log('- Key:', keyStr);
                            console.log('===================');
                        }
                    })
                    .catch((error) => {
                        console.error('=== Load Failed ===');
                        console.error('- Storage Type: Telegram Cloud');
                        console.error('- Key:', keyStr);
                        console.error('- Error:', error);
                        console.error('=================');
                    });
            } catch (error) {
                console.error('=== Error Accessing Telegram Storage ===');
                console.error('- Error:', error);
                console.error('=====================================');
            }
        } else {
            console.warn('=== Telegram WebApp Not Available ===');
            console.warn('Falling back to localStorage');
            try {
                data = localStorage.getItem(keyStr);
                if (data) {
                    console.log('=== Load Successful ===');
                    console.log('- Storage Type: Local Storage');
                    console.log('- Key:', keyStr);
                    console.log('- Data:', data);
                    console.log('- Data Length:', data.length);
                    console.log('=====================');
                } else {
                    console.log('=== No Data Found ===');
                    console.log('- Storage Type: Local Storage');
                    console.log('- Key:', keyStr);
                    console.log('===================');
                }
            } catch (error) {
                console.error('=== Load Failed ===');
                console.error('- Storage Type: Local Storage');
                console.error('- Key:', keyStr);
                console.error('- Error:', error);
                console.error('=================');
            }
        }

        if (!data) {
            return null;
        }

        // Allocate memory for the string and copy it
        var bufferSize = lengthBytesUTF8(data) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(data, buffer, bufferSize);
        return buffer;
    }
}); 