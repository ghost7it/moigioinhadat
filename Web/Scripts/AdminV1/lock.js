var Lock = function () {

    return {
        //main function to initiate the module
        init: function () {

             $.backstretch([
		        "Images/1.jpg",
    		    "Images/2.jpg",
    		    "Images/3.jpg",
    		    "Images/4.jpg"
		        ], {
		          fade: 1000,
		          duration: 8000
		      });
        }

    };

}();