


const showCategoryForm = (isUpdate, data) => {
    const form_wrapper = $('.form-category');

    form_wrapper.find('.form-header').text(isUpdate ? 'Cập nhật danh mục' : 'Tạo danh mục mới');
    form_wrapper.find('form').data('action', isUpdate ? 'update' : 'create');
    form_wrapper.find('#category_id').prop('disabled', isUpdate);
    form_wrapper.find('button[type = "submit"]').text(isUpdate ? 'Cập nhật danh mục' : 'Tạo danh mục');

    if (data) {
        form_wrapper.find('#category_id').val(data.categoryId);
        form_wrapper.find('#category_name').val(data.categoryName);
        form_wrapper.find('.preview-image-box').empty().append(`
            <img src="${data.imageSrc}" alt="" />
        `);
    }

    showForm(form_wrapper);
}


$('.click-add-category').click(() => {
    showCategoryForm(false, null);
})

$('.click-upload-category-image').click(() => {
    $('#category_image').click();
})

$('#category_image').change(function () {
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

$(document).on('click', '.edit-category', function () {
    const category_id = $(this).data('category');

    $.ajax({
        url: `/api/admin/categories/${category_id}`,
        type: 'GET',
        success: (response) => {
            if (response.status) {
                showCategoryForm(true, response.data);
            } else {
                showDialog('error', response.message)
            }
        },
        error: () => {

        }
    })
})

$(document).on('click', '.delete-category', function () {
    const category_id = $(this).data('category');

    showConfirmDialog('Bạn có chắc chắn muốn xóa danh mục này?', 'Danh mục sau khi xóa sẽ không thể khôi phục.', () => {
        showWebLoader();
        $.ajax({
            url: `/api/admin/categories/${category_id}`,
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

$('.form-category form').submit(function (e) {
    e.preventDefault();

    const action = $(this).data('action');
    const api_action = action === 'create' ? 'POST' : 'PUT';

    const category_id = $('#category_id').val();
    const category_name = $('#category_name').val();
    const category_image = $('#category_image')[0].files[0];

    const formData = new FormData();
    formData.append('CategoryId', category_id);
    formData.append('CategoryName', category_name);
    formData.append('image', category_image);

    const btn_submit = $(this).find('button[type="submit"]');
    const btn_element = showButtonLoader(btn_submit, '23px', '4px');

    $.ajax({
        url: `/api/admin/categories`,
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