


const showBrandForm = (isUpdate, data) => {
    const form_wrapper = $('.form-brand');

    form_wrapper.find('.form-header').text(isUpdate ? 'Cập nhật hãng' : 'Thêm mới hãng');
    form_wrapper.find('form').data('action', isUpdate ? 'update' : 'create');
    form_wrapper.find('#brand_id').prop('disabled', isUpdate);
    form_wrapper.find('button[type = "submit"]').text(isUpdate ? 'Cập nhật' : 'Thêm mới');

    if (data) {
        form_wrapper.find('#brand_id').val(data.brandId);
        form_wrapper.find('#brand_name').val(data.brandName);
        form_wrapper.find('#brand_address').val(data.address);
        form_wrapper.find('#brand_phone').val(data.phone);
        form_wrapper.find('.preview-image-box').empty().append(`
            <img src="${data.logoSrc}" alt="" />
        `);
    }

    showForm(form_wrapper);
}


$('.click-add-brand').click(() => {
    showBrandForm(false, null);
})

$('.click-upload-brand-logo').click(() => {
    $('#brand_image').click();
})

$('#brand_image').change(function () {
    const file = $(this)[0].files[0];

    const preview_image = $('.preview-image-box')
    preview_image.empty();

    if (file && file.type.startsWith('image/')) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview_image.append(`
                <img src="${e.target.result}" alt="" />
            `)
        };

        reader.readAsDataURL(file);
    }
})

$(document).on('click', '.edit-brand', function () {
    const brand_id = $(this).data('brand');

    $.ajax({
        url: `/api/admin/brands/${brand_id}`,
        type: 'GET',
        success: (response) => {
            if (response.status) {
                showBrandForm(true, response.data);
            } else {
                showDialog('error', response.message)
            }
        },
        error: () => {

        }
    })
})

$(document).on('click', '.delete-brand', function () {
    const brand_id = $(this).data('brand');

    showConfirmDialog('Bạn có chắc chắn muốn xóa hãng này?', 'Hãng sau khi xóa sẽ không thể khôi phục.', () => {
        showWebLoader();
        $.ajax({
            url: `/api/admin/brands/${brand_id}`,
            type: 'DELETE',
            success: (response) => {
                hideWebLoader(0);
                if (response.status) {
                    showDialogWithCallback('success', response.message, null, () => {
                        location.reload();
                    });
                } else {
                    showDialog('error', response.message)
                }
            },
            error: () => {
                hideWebLoader(0);
                showErrorDialog();
            }
        })
    })
})

$('.form-brand form').submit(function (e) {
    e.preventDefault();

    const action = $(this).data('action');
    const api_action = action === 'create' ? 'POST' : 'PUT';

    const brand_id = $('#brand_id').val();
    const brand_name = $('#brand_name').val();
    const brand_address = $('#brand_address').val();
    const brand_phone = $('#brand_phone').val();
    const brand_image = $('#brand_image')[0].files[0];

    const formData = new FormData();
    formData.append('brandId', brand_id);
    formData.append('brandName', brand_name);
    formData.append('address', brand_address);
    formData.append('phone', brand_phone);
    formData.append('image', brand_image);

    const btn_submit = $(this).find('button[type="submit"]');
    const btn_element = showButtonLoader(btn_submit, '23px', '4px');

    $.ajax({
        url: `/api/admin/brands`,
        type: api_action,
        data: formData,
        contentType: false,
        processData: false,
        success: (response) => {
            if (response.status) {
                showDialogWithCallback('success', response.message, null, () => {
                    location.reload();
                });
            } else {
                showDialog('error', response.message)
            }

            hideButtonLoader(btn_submit, btn_element);
        },
        error: () => {
            hideButtonLoader(btn_submit, btn_element);
        }
    })
})