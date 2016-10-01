var Login = function () {

    var handleLogin = function () {

        $('.login-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: true, // focus/do not focus the last invalid input
            rules: {
                email: {
                    required: true,
                    email: true
                },
                password: {
                    required: true
                },
                remember: {
                    required: false
                }
            },

            messages: {
                email: {
                    required: "Vui lòng nhập địa chỉ email!",
                    email: "Địa chỉ email không đúng định dạng!"
                },
                password: {
                    required: "Vui lòng nhập mật khẩu!"
                }
            },

            invalidHandler: function (event, validator) { //display error alert on form submit   
                $('.alert-danger', $('.login-form')).show();
            },

            highlight: function (element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
                AdminVersionOne.unblockUI($('.content'));
            },

            success: function (label) {
                label.closest('.form-group').removeClass('has-error');
                label.remove();
            },

            errorPlacement: function (error, element) {
                error.insertAfter(element.closest('.input-icon'));
            },

            submitHandler: function (form) {
                form.submit(); // form validation success, call ajax form submit
            }
        });

        $('.login-form input').keypress(function (e) {
            if (e.which == 13) {
                if ($('.login-form').validate().form()) {
                    $('.login-form').submit(); //form validation success, call ajax form submit
                }
                return false;
            }
        });
    }

    var handleForgetPassword = function () {
        //$('.forget-form').validate({
        //    errorElement: 'span', //default input error message container
        //    errorClass: 'help-block', // default input error message class
        //    focusInvalid: false, // do not focus the last invalid input
        //    ignore: "",
        //    rules: {
        //        email_forget: {
        //            required: true,
        //            email: true
        //        }
        //    },

        //    messages: {
        //        email_forget: {
        //            required: "Vui lòng nhập địa chỉ email!",
        //            email: "Địa chỉ email không đúng định dạng!"
        //        }
        //    },

        //    invalidHandler: function (event, validator) { //display error alert on form submit   

        //    },

        //    highlight: function (element) { // hightlight error inputs
        //        $(element)
        //            .closest('.form-group').addClass('has-error'); // set error class to the control group
        //    },

        //    success: function (label) {
        //        label.closest('.form-group').removeClass('has-error');
        //        label.remove();
        //    },

        //    errorPlacement: function (error, element) {
        //        error.insertAfter(element.closest('.input-icon'));
        //    },

        //    submitHandler: function (form) {
        //        form.submit();
        //    }
        //});

        //$('.forget-form input').keypress(function (e) {
        //    if (e.which == 13) {
        //        if ($('.forget-form').validate().form()) {
        //            $('.forget-form').submit();
        //        }
        //        return false;
        //    }
        //});

        jQuery('#forget-password').click(function () {
            $('.alert.alert-danger ').hide();
            $('.has-error').removeClass('has-error');
            $('.help-block').hide();
            jQuery('.login-form').hide();
            jQuery('.forget-form').show();
        });

        jQuery('#back-btn').click(function () {
            $('.alert.alert-danger ').hide();
            $('.has-error').removeClass('has-error');
            $('.help-block').hide();
            jQuery('.login-form').show();
            jQuery('.forget-form').hide();
        });

    }
    return {
        init: function () {

            handleLogin();
            handleForgetPassword();
        }

    };

}();