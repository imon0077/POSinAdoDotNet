/**
 * Custom js on 9/2/15.
 */


(function ($) {

    'use strict';
    
    $("#flash-messages").flashMessage();

    //$('.DeleteData').on('click', function() {
    //    alert("hello");
    //});

    //start : delete
    $('.DeleteData').confirmation({
        placement: 'left',
        onConfirm: function (event, e) {

            var ItemId;
            ItemId = $(e[0]).attr('data-value');

            $.ajax({
                type: "POST",
                url: "prcDeleteData",
                data: { ItemId: ItemId },
                success: function (data) {
                    if (data=="1") {

                        new PNotify({
                            title: 'Notification',
                            text: 'Data Deleted Successfully.',
                            type: 'custom',
                            addclass: 'notification-success',
                            icon: 'fa fa-trash'
                        });

                        window.setTimeout(function () {
                            window.location.href = 'Index';
                        }, 2000);

                    }
                    else {
                        new PNotify({
                            title: 'Notification',
                            text: 'Something went wrong',
                            type: 'custom',
                            addclass: 'notification-danger',
                            icon: 'fa fa-trash'
                        });
                    }

                }

            });

        },
        onCancel: function () {
            new PNotify({
                title: 'Notification',
                text: 'Request cancelled',
                type: 'custom',
                addclass: 'notification-warning',
                icon: 'fa fa-check'
            });
        }
    });

    //end :  dept delete

   

    /*
	Notifications Position
	*/
    var stack_topleft = { "dir1": "down", "dir2": "right", "push": "top" };
    var stack_bottomleft = { "dir1": "right", "dir2": "up", "push": "top" };
    var stack_bottomright = { "dir1": "up", "dir2": "left", "firstpos1": 15, "firstpos2": 15 };
    var stack_bar_top = { "dir1": "down", "dir2": "right", "push": "top", "spacing1": 0, "spacing2": 0 };
    var stack_bar_bottom = { "dir1": "up", "dir2": "right", "spacing1": 0, "spacing2": 0 };
    /* */

    
    // Department form validation
    $("#Department").validate({
        highlight: function (label) {
            $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
        },
        success: function (label) {
            $(label).closest('.form-group').removeClass('has-error');
            label.remove();
        },
        errorPlacement: function (error, element) {
            var placement = element.closest('.input-group');
            if (!placement.get(0)) {
                placement = element;
            }
            if (error.text() !== '') {
                placement.after(error);
            }
        }
    });



    /*
	 Start : Employee Edit
	*/
    /* end : Maritial Status check */
    $('#MaritalSts').on('click', function (e) {
        e.preventDefault();
        var MaritialStatus = $("#MaritalSts").find("option:selected").prop("value");

        if (MaritialStatus == 'Married') {
            $('#EmpSpouse').attr('disabled', false);
            new PNotify({
                title: 'Notification',
                text: 'Please Enter Spouse Name.',
                type: 'warning',
                addclass: 'stack-bar-top',
                stack: stack_bar_top,
                width: "100%"
            });
        }
        else {
            $('#EmpSpouse').attr('disabled', true);
        }

    });

    var MaritialStatus = $("#MaritalSts").find("option:selected").prop("value");
    if (MaritialStatus == 'Married') {
        $('#EmpSpouse').attr('disabled', false);
    }
    else {
        $('#EmpSpouse').attr('disabled', true);
    }
    
    /* end : Maritial Status check */


    /*
	Employee Edit Wizard #w4
	*/
    var $w4finish = $('#w4').find('ul.pager li.finish'),
		$w4validator = $("#w4 form").validate({
		    highlight: function (element) {
		        $(element).closest('.form-group').removeClass('has-success').addClass('has-error');
		    },
		    success: function (element) {
		        $(element).closest('.form-group').removeClass('has-error');
		        $(element).remove();
		    },
		    errorPlacement: function (error, element) {
		        element.parent().append(error);
		    }
		});

    $w4finish.on('click', function (ev) {
        ev.preventDefault();
        var validated = $('#w4 form').valid();
        if (validated) {

            $('#EditEmployee').submit();

            new PNotify({
                title: 'Congratulations',
                text: 'Your form submitted successfully.',
                type: 'custom',
                addclass: 'notification-success',
                icon: 'fa fa-check'
            });
        }
    });

    $('#w4').bootstrapWizard({
        tabClass: 'wizard-steps',
        nextSelector: 'ul.pager li.next',
        previousSelector: 'ul.pager li.previous',
        firstSelector: null,
        lastSelector: null,
        onNext: function (tab, navigation, index, newindex) {
            var validated = $('#w4 form').valid();
            if (!validated) {
                $w4validator.focusInvalid();
                return false;
            }
        },
        onTabClick: function (tab, navigation, index, newindex) {
            if (newindex == index + 1) {
                return this.onNext(tab, navigation, index, newindex);
            } else if (newindex > index + 1) {
                return false;
            } else {
                return true;
            }
        },
        onTabChange: function (tab, navigation, index, newindex) {
            var $total = navigation.find('li').size() - 1;
            $w4finish[newindex != $total ? 'addClass' : 'removeClass']('hidden');
            $('#w4').find(this.nextSelector)[newindex == $total ? 'addClass' : 'removeClass']('hidden');
        },
        onTabShow: function (tab, navigation, index) {
            var $total = navigation.find('li').length - 1;
            var $current = index;
            var $percent = Math.floor(($current / $total) * 100);
            $('#w4').find('.progress-indicator').css({ 'width': $percent + '%' });
            tab.prevAll().addClass('completed');
            tab.nextAll().removeClass('completed');
        }
    });

    /* end : widget */


    
    /* start : Education Qualification */

    //start :add more field
    $('#btnAdd').on('click', function () {
        var num     = $('.clonedInput').length, // Checks to see how many "duplicatable" input fields we currently have
            
            newNum = new Number(num + 1),      // The numeric ID of the new input field being added, increasing by 1 each time
            newRow = new Number(num ),
            newElem = $('#entry' + num).clone().attr('id', 'entry' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
        
        // ExamName - select | option | Combo
        newElem.find('.ExamName').parent().attr('for', 'ID' + newNum + 'ExamName');
        newElem.find('.ExamName').attr('id', 'ID' + newNum + 'ExamName').attr('name', 'EmpEdu[' + newRow + '].ExamName').val('');

        // MajorSub - select | option | Combo
        newElem.find('.MajorSub').parent().attr('for', 'ID' + newNum + 'MajorSub');
        newElem.find('.MajorSub').attr('id', 'ID' + newNum + 'MajorSub').attr('name', 'EmpEdu[' + newRow + '].MajorSub').val('');

        // BoardName - select | option | Combo
        newElem.find('.BoardName').parent().attr('for', 'ID' + newNum + 'BoardName');
        newElem.find('.BoardName').attr('id', 'ID' + newNum + 'BoardName').attr('name', 'EmpEdu[' + newRow + '].BoardName').val('');

        // InstituteName - text
        newElem.find('.InstituteName').parent().attr('for', 'ID' + newNum + 'InstituteName');
        newElem.find('.InstituteName').attr('id', 'ID' + newNum + 'InstituteName').val('').attr('name', 'EmpEdu[' + newRow + '].InstituteName').val('');

        // ExamResult - text
        newElem.find('.ExamResult').parent().attr('for', 'ID' + newNum + 'ExamResult');
        newElem.find('.ExamResult').attr('id', 'ID' + newNum + 'ExamResult').val('').attr('name', 'EmpEdu[' + newRow + '].ExamResult').val('');

        // PassingYear - text
        newElem.find('.PassingYear').parent().attr('for', 'ID' + newNum + 'PassingYear');
        newElem.find('.PassingYear').attr('id', 'ID' + newNum + 'PassingYear').val('').attr('name', 'EmpEdu[' + newRow + '].PassingYear').val('');

        // Insert the new element after the last "duplicatable" input field
        $('#entry' + num).after(newElem);
        $('#entry' + newNum).focus();

        // Enable the "remove" button. This only shows once you have a duplicated row.
        $('#btnDel').attr('disabled', false);
        
        if (newNum == 10)
            $('#btnAdd').attr('disabled', true).prop('value', "You've reached the limit"); // value here updates the text in the 'add' button when the limit is reached
    });

    
    //Delete a row
    $('#btnDel').confirmation({
        placement: 'left',
        onConfirm: function () {
            
            var num = $('.clonedInput').length;

            $('#entry' + num).slideUp('slow', function () {
                $(this).remove();
                // if only one element remains, disable the "remove" button
                if (num - 1 === 1)
                    $('#btnDel').attr('disabled', true);
                // enable the "add" button
                $('#btnAdd').attr('disabled', false).prop('value', "+");
            });

            new PNotify({
                title: 'Notification',
                text: 'One row removed',
                type: 'custom',
                addclass: 'notification-danger',
                icon: 'fa fa-check'
            });

        },
        onCancel: function () {
            new PNotify({
                title: 'Notification',
                text: 'Request cancelled',
                type: 'custom',
                addclass: 'notification-warning',
                icon: 'fa fa-check'
            });
        }
    });

    $('#btnAppEdu').on('click', function () {
        var num = $('.clonedInput').length, // Checks to see how many "duplicatable" input fields we currently have

            newNum = new Number(num + 1),      // The numeric ID of the new input field being added, increasing by 1 each time
            newRow = new Number(num),
            newElem = $('#entry' + num).clone().attr('id', 'entry' + newNum).fadeIn('slow'); // create the new element via clone(), and manipulate it's ID using newNum value
        //alert(newNum + "" + newRow);
        // ExamName - select | option | Combo
        newElem.find('.ExamName').parent().attr('for', 'ID' + newNum + 'ExamName');
        newElem.find('.ExamName').attr('id', 'ID' + newNum + 'ExamName').attr('name', 'ApplicantEdu[' + newRow + '].ExamName').val('');

        // MajorSub - select | option | Combo
        newElem.find('.MajorSub').parent().attr('for', 'ID' + newNum + 'MajorSub');
        newElem.find('.MajorSub').attr('id', 'ID' + newNum + 'MajorSub').attr('name', 'ApplicantEdu[' + newRow + '].MajorSub').val('');

        // BoardName - select | option | Combo
        newElem.find('.BoardName').parent().attr('for', 'ID' + newNum + 'BoardName');
        newElem.find('.BoardName').attr('id', 'ID' + newNum + 'BoardName').attr('name', 'ApplicantEdu[' + newRow + '].BoardName').val('');

        // InstituteName - text
        newElem.find('.InstituteName').parent().attr('for', 'ID' + newNum + 'InstituteName');
        newElem.find('.InstituteName').attr('id', 'ID' + newNum + 'InstituteName').val('').attr('name', 'ApplicantEdu[' + newRow + '].InstituteName').val('');

        // ExamResult - text
        newElem.find('.ExamResult').parent().attr('for', 'ID' + newNum + 'ExamResult');
        newElem.find('.ExamResult').attr('id', 'ID' + newNum + 'ExamResult').val('').attr('name', 'ApplicantEdu[' + newRow + '].ExamResult').val('');

        // PassingYear - text
        newElem.find('.PassingYear').parent().attr('for', 'ID' + newNum + 'PassingYear');
        newElem.find('.PassingYear').attr('id', 'ID' + newNum + 'PassingYear').val('').attr('name', 'ApplicantEdu[' + newRow + '].PassingYear').val('');

        // Insert the new element after the last "duplicatable" input field
        $('#entry' + num).after(newElem);
        $('#entry' + newNum).focus();

        // Enable the "remove" button. This only shows once you have a duplicated row.
        $('#btnDelAppEdu').attr('disabled', false);

        if (newNum == 10)
            $('#btnAppEdu').attr('disabled', true).prop('value', "You've reached the limit"); // value here updates the text in the 'add' button when the limit is reached
    });


    //Delete a row
    $('#btnDelAppEdu').confirmation({
        placement: 'left',
        onConfirm: function () {

            var num = $('.clonedInput').length;

            $('#entry' + num).slideUp('slow', function () {
                $(this).remove();
                // if only one element remains, disable the "remove" button
                if (num - 1 === 1)
                    $('#btnDelAppEdu').attr('disabled', true);
                // enable the "add" button
                $('#btnAppEdu').attr('disabled', false).prop('value', "+");
            });

            new PNotify({
                title: 'Notification',
                text: 'One row removed',
                type: 'custom',
                addclass: 'notification-danger',
                icon: 'fa fa-check'
            });

        },
        onCancel: function () {
            new PNotify({
                title: 'Notification',
                text: 'Request cancelled',
                type: 'custom',
                addclass: 'notification-warning',
                icon: 'fa fa-check'
            });
        }
    });

    var numRows = $('.clonedInput').length;
    
    // Enable the "add" button
    $('#btnAdd').attr('disabled', false);
    $('#btnAppEdu').attr('disabled', false);

    if (numRows > 1) {
        // Enable the "remove" button
        $('#btnDel').attr('disabled', false);
        $('#btnDelAppEdu').attr('disabled', false);
    }
    else {
        // Disable the "remove" button
        $('#btnDel').attr('disabled', true);
        $('#btnDelAppEdu').attr('disabled', true);
    }
    

  //end :add more field

 /* End : Education Qualification */


 /* start : Employment History */

    //start :add more field
    $('#btnAddExp').on('click', function () {
        var num = $('.clonedInputExp').length, 

            newNum = new Number(num + 1),      
            newRow = new Number(num),
            newElem = $('#entryExp' + num).clone().attr('id', 'entryExp' + newNum).fadeIn('slow'); 

        // PrevCom - text
        newElem.find('.PrevCom').parent().attr('for', 'ID' + newNum + 'PrevCom');
        newElem.find('.PrevCom').attr('id', 'ID' + newNum + 'PrevCom').attr('name', 'EmpExp[' + newRow + '].PrevCom').val('');

        // PrevDesig - text
        newElem.find('.PrevDesig').parent().attr('for', 'ID' + newNum + 'PrevDesig');
        newElem.find('.PrevDesig').attr('id', 'ID' + newNum + 'PrevDesig').attr('name', 'EmpExp[' + newRow + '].PrevDesig').val('');

        // PrevSalary - text
        newElem.find('.PrevSalary').parent().attr('for', 'ID' + newNum + 'PrevSalary');
        newElem.find('.PrevSalary').attr('id', 'ID' + newNum + 'PrevSalary').attr('name', 'EmpExp[' + newRow + '].PrevSalary').val('');

        newElem.find('.dtFrom').datepicker('destroy').datepicker({
            autoclose: true
        });

        newElem.find('.dtTo').datepicker('destroy').datepicker({
            autoclose: true
        });

        // dtFrom - text
        newElem.find('.dtFrom').parent().attr('for', 'ID' + newNum + 'dtFrom');
        newElem.find('.dtFrom').attr('id', 'ID' + newNum + 'dtFrom').val('').attr('name', 'EmpExp[' + newRow + '].dtFrom').val('');

        // dtTo - text
        newElem.find('.dtTo').parent().attr('for', 'ID' + newNum + 'dtTo');
        newElem.find('.dtTo').attr('id', 'ID' + newNum + 'dtTo').val('').attr('name', 'EmpExp[' + newRow + '].dtTo').val('');

        // Remarks - text
        newElem.find('.Remarks').parent().attr('for', 'ID' + newNum + 'Remarks');
        newElem.find('.Remarks').attr('id', 'ID' + newNum + 'Remarks').val('').attr('name', 'EmpExp[' + newRow + '].Remarks').val('');

        $('#entryExp' + num).after(newElem);
        $('#entryExp' + newNum).focus();

        $('#btnDelExp').attr('disabled', false);

        if (newNum == 15)
            $('#btnAddExp').attr('disabled', true).prop('value', "You've reached the limit"); 
    });


    //Delete a row
    $('#btnDelExp').confirmation({
        placement: 'left',
        onConfirm: function () {

            var num = $('.clonedInputExp').length;

            $('#entryExp' + num).slideUp('slow', function () {
                $(this).remove();
                
                if (num - 1 === 1)
                    $('#btnDelExp').attr('disabled', true);
                
                $('#btnAddExp').attr('disabled', false).prop('value', "+");
            });

            new PNotify({
                title: 'Notification',
                text: 'One row removed',
                type: 'custom',
                addclass: 'notification-danger',
                icon: 'fa fa-check'
            });

        },
        onCancel: function () {
            new PNotify({
                title: 'Notification',
                text: 'Request cancelled',
                type: 'custom',
                addclass: 'notification-warning',
                icon: 'fa fa-check'
            });
        }
    });

    $('#btnAddAppExp').on('click', function () {
        var num = $('.clonedInputExp').length,

            newNum = new Number(num + 1),
            newRow = new Number(num),
            newElem = $('#entryExp' + num).clone().attr('id', 'entryExp' + newNum).fadeIn('slow');

        // PrevCom - text
        newElem.find('.PrevCom').parent().attr('for', 'ID' + newNum + 'PrevCom');
        newElem.find('.PrevCom').attr('id', 'ID' + newNum + 'PrevCom').attr('name', 'ApplicantExp[' + newRow + '].PrevCom').val('');

        // PrevDesig - text
        newElem.find('.PrevDesig').parent().attr('for', 'ID' + newNum + 'PrevDesig');
        newElem.find('.PrevDesig').attr('id', 'ID' + newNum + 'PrevDesig').attr('name', 'ApplicantExp[' + newRow + '].PrevDesig').val('');

        // PrevSalary - text
        newElem.find('.PrevSalary').parent().attr('for', 'ID' + newNum + 'PrevSalary');
        newElem.find('.PrevSalary').attr('id', 'ID' + newNum + 'PrevSalary').attr('name', 'ApplicantExp[' + newRow + '].PrevSalary').val('');

        newElem.find('.dtFrom').datepicker('destroy').datepicker({
            autoclose: true
        });

        newElem.find('.dtTo').datepicker('destroy').datepicker({
            autoclose: true
        });

        // dtFrom - text
        newElem.find('.dtFrom').parent().attr('for', 'ID' + newNum + 'dtFrom');
        newElem.find('.dtFrom').attr('id', 'ID' + newNum + 'dtFrom').val('').attr('name', 'ApplicantExp[' + newRow + '].dtFrom').val('');

        // dtTo - text
        newElem.find('.dtTo').parent().attr('for', 'ID' + newNum + 'dtTo');
        newElem.find('.dtTo').attr('id', 'ID' + newNum + 'dtTo').val('').attr('name', 'ApplicantExp[' + newRow + '].dtTo').val('');

        // Remarks - text
        newElem.find('.Remarks').parent().attr('for', 'ID' + newNum + 'Remarks');
        newElem.find('.Remarks').attr('id', 'ID' + newNum + 'Remarks').val('').attr('name', 'ApplicantExp[' + newRow + '].Remarks').val('');

        $('#entryExp' + num).after(newElem);
        $('#entryExp' + newNum).focus();

        $('#btnDelAppExp').attr('disabled', false);

        if (newNum == 15)
            $('#btnAddAppExp').attr('disabled', true).prop('value', "You've reached the limit");
    });


    //Delete a row
    $('#btnDelAppExp').confirmation({
        placement: 'left',
        onConfirm: function () {

            var num = $('.clonedInputExp').length;

            $('#entryExp' + num).slideUp('slow', function () {
                $(this).remove();

                if (num - 1 === 1)
                    $('#btnDelAppExp').attr('disabled', true);

                $('#btnAddAppExp').attr('disabled', false).prop('value', "+");
            });

            new PNotify({
                title: 'Notification',
                text: 'One row removed',
                type: 'custom',
                addclass: 'notification-danger',
                icon: 'fa fa-check'
            });

        },
        onCancel: function () {
            new PNotify({
                title: 'Notification',
                text: 'Request cancelled',
                type: 'custom',
                addclass: 'notification-warning',
                icon: 'fa fa-check'
            });
        }
    });



    var numRowsExp = $('.clonedInputExp').length;

    // Enable the "add" button
    $('#btnAddExp').attr('disabled', false);
    $('#btnAddAppExp').attr('disabled', false);

    if (numRowsExp > 1) {
        // Enable the "remove" button
        $('#btnDelExp').attr('disabled', false);
        $('#btnDelAppExp').attr('disabled', false);
    }
    else {
        // Disable the "remove" button
        $('#btnDelExp').attr('disabled', true);
        $('#btnDelAppExp').attr('disabled', true);
    }


    //end :add more field

 /* End : Employment History */








    $('#join_validate').click(function(){
        var captchaForm = $(this).closest('form');
        var captchaData = captchaForm.serialize();
        var formData = $('#join_form').serialize();

        $.ajax({
            url:"captcha",
            data: captchaData,
            type:"post"
        }).done(function(e){
                if(e == 'Y'){
                    //alert(e);
                    $.ajax({
                        url:"job-app-submit",
                        data: formData,
                        type:"post"
                    }).done(function(e){
                            $('.close').click();
                            if(e == 'Done'){
                                $('#captchaModalValidate').modal();
                            }
                            else{
                                $('#unsuccess').modal();
                            }
                        });
                }
                else{
                    $.ajax({
                        url:"captcha",
                        type:"get"
                    }).done(function(e){
                            $('.captcha_modal').html(e);
                            $('#CaptchaCode').val('');
                        });
                }
            });
    });


$('#RequestContact').click(function(){
        var removeForm = $(this).closest('form');
        var formData = removeForm.serialize();

            $.ajax({
                url:"remove-submit",
                data: formData,
                type:"post"
            }).done(function(e){
                    $('.close').click();
                    if(e == 'Done'){
                        $('#removeModalValidate').modal();
                    }
                    else{
                        alert('There is a problem to remove!');
                    }
                });
    });

    $('.dismiss_modal').click(function(){
        window.location="index";
    });

    $('#gardenCleaning').click(function(){
        $('#garden_no').hide();
        $('#houseCleaning').removeAttr('checked');
    });
    $('#houseCleaning').click(function(){
        $('#garden_no').show();
        $('#gardenCleaning').removeAttr('checked');
    });

    $('#repair').click(function(){
        $('#building_size').hide();
        $('#building').removeAttr('checked');
    });
    $('#building').click(function(){
        $('#building_size').show();
        $('#repair').removeAttr('checked');
    });

    $('.yes').click(function(){
        $('.no').removeAttr('checked');
        $('#company').show();
    });
    $('.no').click(function(){
        $('.yes').removeAttr('checked');
        $('#company').hide();
    });

    $('.yes2').click(function(){
        $('.no2').removeAttr('checked');
    });
    $('.no2').click(function(){
        $('.yes2').removeAttr('checked');
    });

    $('.from').datepicker({
        format:'dd MM yyyy',
        autoclose: true
    });
    $('.to').datepicker({
        format:'dd MM yyyy',
        autoclose: true
    });


}).apply(this, [jQuery]);
