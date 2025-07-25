// Function để bật/tắt Dark Mode (đã có)
function toggleDarkMode() {
    document.body.classList.toggle('dark');
    localStorage.setItem('darkMode', document.body.classList.contains('dark'));
}

// Kiểm tra trạng thái Dark Mode khi tải trang (đã có)
if (localStorage.getItem('darkMode') === 'true') {
    document.body.classList.add('dark');
}

$(document).ready(function () {
    // --- Logic tìm kiếm (đã có từ các phiên bản trước) ---
    const searchInput = $('#searchInput');
    const searchSuggestions = $('#searchSuggestions');

    searchInput.on('input', function () {
        const query = $(this).val().trim();
        if (query.length > 0) {
            $.ajax({
                url: '@Url.Action("Search", "Pro")', // Đảm bảo action này tồn tại và trả về JSON
                type: 'GET',
                data: { q: query },
                success: function (response) {
                    searchSuggestions.empty();
                    if (response.length > 0) {
                        response.forEach(item => {
                            // Đảm bảo item có các thuộc tính khớp với Product Model của bạn (ví dụ: namePro, Image, Price, _id)
                            if (item._id && item.namePro) {
                                const suggestion = `
                                    <a href="@Url.Action("Detail", "Pro", new { id = "${item._id}" })" class="block px-4 py-2 text-gray-800 dark:text-gray-200 hover:bg-blue-600 hover:text-white flex items-center">
                                        <img src="${item.Image || '@Url.Content("~/images/noimage.png")'}" alt="${item.namePro}" class="w-10 h-10 object-cover mr-2" onerror="this.onerror=null;this.src='@Url.Content("~/images/noimage.png")';">
                                        <div>
                                            <div class="font-bold">${item.namePro}</div>
                                            <div class="text-sm text-gray-500 dark:text-gray-400">${item.Price ? item.Price.toLocaleString('vi-VN') + ' VNĐ' : 'Chưa có giá'}</div>
                                        </div>
                                    </a>
                                `;
                                searchSuggestions.append(suggestion);
                            } else {
                                console.warn('Dữ liệu item không đầy đủ hoặc sai định dạng:', item);
                            }
                        });
                        searchSuggestions.removeClass('hidden');
                    } else {
                        searchSuggestions.addClass('hidden');
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Error:', error, xhr.responseText);
                    searchSuggestions.addClass('hidden');
                }
            });
        } else {
            searchSuggestions.addClass('hidden');
        }
    });

    $(document).on('click', function (e) {
        if (!searchInput.is(e.target) && !searchSuggestions.is(e.target) && searchSuggestions.has(e.target).length === 0) {
            searchSuggestions.addClass('hidden');
        }
    });

    // --- LOGIC MỚI: Dropdown Menu cho Category ---
    // Sử dụng jQuery cho sự kiện hover để hiện/ẩn class 'hidden'
    const categoryMenu = $('.category-menu');
    const categoryDropdown = categoryMenu.find('.dropdown-menu');

    if (categoryMenu.length) {
        categoryMenu.hover(
            function () {
                categoryDropdown.removeClass('hidden'); // Hiển thị dropdown khi di chuột vào
            },
            function () {
                categoryDropdown.addClass('hidden'); // Ẩn dropdown khi di chuột ra
            }
        );
    }

    // --- LOGIC MỚI: Dropdown Menu cho User Profile ---
    // Sử dụng jQuery cho sự kiện hover để hiện/ẩn class 'hidden'
    const userProfileMenu = $('.user-profile-menu');
    const userDropdownMenu = userProfileMenu.find('.user-dropdown-menu');

    if (userProfileMenu.length) {
        userProfileMenu.hover(
            function () {
                userDropdownMenu.removeClass('hidden'); // Hiển thị dropdown khi di chuột vào
            },
            function () {
                userDropdownMenu.addClass('hidden'); // Ẩn dropdown khi di chuột ra
            }
        );

        // Optional: Đóng dropdown khi click ra ngoài để tránh bị kẹt
        $(document).on('click', function (event) {
            // Kiểm tra nếu click không phải bên trong userProfileMenu và dropdown đang mở
            if (!userProfileMenu.is(event.target) && userProfileMenu.has(event.target).length === 0 && !userDropdownMenu.hasClass('hidden')) {
                userDropdownMenu.addClass('hidden');
            }
        });
    }

}); // Kết thúc document.ready
$(document).ready(function () {
    // --- Logic tìm kiếm (Đã được chuyển vào scripts.js) ---
    const searchInput = $('#searchInput');
    const searchSuggestions = $('#searchSuggestions');

    searchInput.on('input', function () {
        const query = $(this).val().trim();
        if (query.length > 0) {
            $.ajax({
                url: '@Url.Action("Search", "Search")', // <--- ĐÃ SỬA URL ĐẾN SEARCHCONTROLLER
                type: 'GET',
                data: { q: query },
                success: function (response) {
                    searchSuggestions.empty();
                    if (response.length > 0) {
                        response.forEach(item => {
                            const suggestion = `
                                        <a href="@Url.Action("Detail", "Pro", new { id = "${item._id}" })" class="block px-4 py-2 text-gray-800 dark:text-gray-200 hover:bg-blue-600 hover:text-white flex items-center">
                                            <img src="${item.Image || '@Url.Content("~/images/noimage.png")'}" alt="${item.namePro}" class="w-10 h-10 object-cover mr-2" onerror="this.onerror=null;this.src='@Url.Content("~/images/noimage.png")';">
                                            <div>
                                                <div class="font-bold">${item.namePro}</div>
                                                <div class="text-sm text-gray-500 dark:text-gray-400">${item.Price ? item.Price.toLocaleString('vi-VN') + ' VNĐ' : 'Chưa có giá'}</div>
                                            </div>
                                        </a>
                                    `;
                            searchSuggestions.append(suggestion);
                        });
                        searchSuggestions.removeClass('hidden');
                    } else {
                        searchSuggestions.addClass('hidden');
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Error:', error, xhr.responseText);
                    searchSuggestions.addClass('hidden');
                }
            });
        } else {
            searchSuggestions.addClass('hidden');
        }
    });

    $(document).on('click', function (e) {
        if (!searchInput.is(e.target) && !searchSuggestions.is(e.target) && searchSuggestions.has(e.target).length === 0) {
            searchSuggestions.addClass('hidden');
        }
    });

    // --- LOGIC Dropdown Menu cho Category và User Profile (Đã được chuyển vào scripts.js) ---
    // NẾU CÓ BẤT KỲ JS NÀO Ở ĐÂY, HÃY CHUYỂN NÓ VÀO wwwroot/Layout/scripts.js
});