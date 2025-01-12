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
    }
});