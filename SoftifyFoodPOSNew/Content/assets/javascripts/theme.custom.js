function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for(var i=0;i < ca.length;i++) {
        var c = ca[i];
        while (c.charAt(0)==' ') c = c.substring(1,c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
    }
    return null;
}

$('.sidebar-toggle').click(function(){
    var me = readCookie('currentView');
    document.cookie = (me == 'max' || me == null) ? 'currentView=min': 'currentView=max';
    //console.log(me);
});

$(document).ready(function(){
    var me = readCookie('currentView');
    if(me == 'min'){
        document.cookie = 'currentView=max';
        $('.sidebar-toggle').click();
    }
});

/**
 * Autoload town list
 * Custom | Date: 25.12.2015
 */
$(document).ready(function(){
    $('.town').keyup(function(){
        var townVal = $(this).val();
        var csrf = $('#csrf').val();

        var ThisDiv = $(this).parent('div').children('div');
        var ThisUl = ThisDiv.children('ul');

        if(townVal.length > 2){
            $.ajax({
                url:"town-submit",
                data: {townVal:townVal, _token:csrf},
                type:"post"
            }).done(function(e){
                ThisDiv.show();
                ThisUl.html(e);
            });
        }
        else{
            ThisDiv.hide();
            ThisUl.html('');
        }

    });

    $('.town').blur(function(){
        var ThisDiv = $(this).parent('div').children('div');
        var ThisUl = ThisDiv.children('ul');

        ThisDiv.hide();
        ThisUl.html('');
    });

    $('.town-ul').delegate('.city','mouseover',function(){
        var cityVal = $(this).text();
        //console.log(cityVal);
        var ThisInput = $(this).closest('.ville').children('input.town');
        ThisInput.val(cityVal);
    });
});