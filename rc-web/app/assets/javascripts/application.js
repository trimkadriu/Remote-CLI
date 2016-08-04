// This is a manifest file that'll be compiled into application.js, which will include all the files
// listed below.
//
// Any JavaScript/Coffee file within this directory, lib/assets/javascripts, vendor/assets/javascripts,
// or any plugin's vendor/assets/javascripts directory can be referenced here using a relative path.
//
// It's not advisable to add code directly here, but if you do, it'll appear at the bottom of the
// compiled file. JavaScript code in this file should be added after the last require_* statement.
//
// Read Sprockets README (https://github.com/rails/sprockets#sprockets-directives) for details
// about supported directives.
//
//= require jquery
//= require jquery_ujs
//= require bootstrap.min
//= require turbolinks
//= require bootstrap-sprockets
//= require_tree ./channels
//= require machines
//= #require_tree .
$(document).ready(function() {
    // if there is an .alert element on the current page
    if($(".alert").length){
        // remove it after 3 seconds
        setTimeout(function(){
            $(".alert").slideUp(function(){
                $(this).remove();
            });
        }, 3000);
    }
    
    // Styling forms
    if($('form').length){
        $(".field").addClass('form-group');
        $(".field input, .field textarea, .field select").addClass('form-control');

        $(".field input[type='checkbox'], .field input[type='radio']").each(function(){
            $(this).removeClass('form-control');

            var fieldType = $(this).attr('type').toLowerCase();
            var $parent = $(this).parent();
            $parent.find('br').remove();
            var thisParentHTML = $parent.html();
            $parent.find('label').remove();
            var checkboxHTML = $parent.html()
            $parent.html('<div class="'+fieldType+'">'+thisParentHTML+'</div>');
            $parent.find('input').remove();
            $parent.find('label').prepend(checkboxHTML);
        });

        $("form .actions").addClass('btn-group');
        $("form input[type='submit']").addClass('btn btn-teal');
    }

});
