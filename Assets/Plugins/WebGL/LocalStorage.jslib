mergeInto(LibraryManager.library, {
    SaveToLocal: function(key, data) {
        var keyStr = UTF8ToString(key);
        var dataStr = UTF8ToString(data);
        
        console.log('=== Saving to Local Storage ===');
        console.log('Key:', keyStr);
        console.log('Data:', dataStr);
        
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
    },

    LoadFromLocal: function(key) {
        var keyStr = UTF8ToString(key);
        var data = null;
        
        console.log('=== Loading from Local Storage ===');
        console.log('Key:', keyStr);
        
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

        if (!data) {
            return null;
        }

        var bufferSize = lengthBytesUTF8(data) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(data, buffer, bufferSize);
        return buffer;
    }
}); 