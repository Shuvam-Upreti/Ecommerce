 $(document).ready(function () {
	 $('#sidebarCollapse').on('click', function () {
            $('#sidebar,#content').toggleClass('active');			
         $(".overlay").css({ "display": "block" });
        });
		$('.overlay' ).on('click', function () {
			
				 $('#sidebar,#content').addClass('active');
				 $(".overlay").css({"display": "none"});
			
        });
     /*tooltip*/
     $('[data-toggle="tooltip"]').tooltip();
	   })