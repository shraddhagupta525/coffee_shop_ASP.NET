// Function to send delete request
function deleteProduct(productId) {
    const url = `http://localhost:63946/api/product/deleteproduct/${productId}`;

    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
            // Add any other headers as needed
        }
        // You can include a body if needed for DELETE requests, but it's often not necessary
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok.');
            }
            // Handle success response if needed
            console.log('Product deleted successfully.');
            window.location.reload();
            // Optionally, you can update the UI or reload data after successful deletion
        })
        .catch(error => {
            console.error('Error deleting product:', error);
            // Handle error scenario, show error message, etc.
        });
}

// sapate product
function sapareteProd(products) {
    products = JSON.parse(products).products
    var htmldata = "";
    //console.log(products.products)
    for (var i = 0; i < products.length; i++) {
        htmldata += `<p>${products[i].quantity} ${products[i].name}</p><br/>`
    }
    // console.log(htmldata)
    return htmldata;
}



// fetch users from http://localhost:63946/api/product/getAllProduct and add in table of customers
    fetch("http://localhost:63946/api/product/getAllProduct").then(response => response.json())
        .then(data => {
            data = Array.from(Object.values(data));
            const table = document.querySelector('section table tbody');
            data.forEach(product => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                        <td>${product.id}</td>
                        <td>${product.name}</td>
                        <td>${product.description}</td>
                        <td>${product.price}</td>
                        <td>${product.status}</td>
                        <td>${product.categoryName}</td>
                        <td><button class="delete-button" data-key="${product.id}">Delete<button/></td>
                    `;
                table.appendChild(tr);
            });

            const deleteButtons = document.querySelectorAll('.delete-button')
            console.log(deleteButtons)
            deleteButtons.forEach(button => {
                button.addEventListener('click', () => {
                    const productId = button.getAttribute('data-key');
                    if (productId) {
                        deleteProduct(productId);
                    } else {
                        console.error('Product ID not found.');
                    }
                });
            });
        });

    // fetch orders from http://localhost:63946/api/bill/getBills and add in table of orders
    fetch('http://localhost:63946/api/bill/getBills')
        .then(response => response.json())
        .then(data => {
            data = Array.from(Object.values(data));
            const table = document.querySelectorAll('section table tbody')[1];
            table.innerHTML = '';
            data.forEach(order => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                        <td>${order.id}</td>
                        <td>${order.uuid}</td>
                        <td>${order.name}</td>
                        <td>${order.email}</td>
                        <td>${order.contactNumber}</td>
                        <td>${order.paymentMethod}</td>
                        <td>${order.totalAmount}</td>
                        <td>${sapareteProd(order.productDetails)}</td>
                        <td>${order.createdBy}</td>
                    `;
                table.appendChild(tr);
            });
        });

    // fetch customers from http://localhost:63946/api/user/getAllUser and add in table of customers
    fetch('http://localhost:63946/api/user/getAllUser')
        .then(response => response.json())
        .then(data => {
            data = Array.from(Object.values(data));
            const table = document.querySelectorAll('section table tbody')[2];
            table.innerHTML = '';
            data.forEach(customer => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                        <td>${customer.id}</td>
                        <td>${customer.name}</td>
                        <td>${customer.contactNumber}</td>
                        <td>${customer.email}</td>
                        <td>${customer.status}</td>
                        <td>${customer.role}</td>
                    `;
                table.appendChild(tr);
            });
        });

window.addEventListener('DOMContentLoaded', () => {
    document.getElementById('productBtn').click()

    const navLinks = document.querySelectorAll('header nav ul li a');
    navLinks.forEach(link => {
        link.addEventListener('click', () => {
            navLinks.forEach(link => {
                link.classList.remove('active');
            });
            link.classList.add('active');
        });
    });

    const openFormButton = document.getElementById('openForm');
    const overlay = document.getElementById('overlay');
    const closeOverlay = document.querySelector('.close');
    const productForm = document.getElementById('productForm');
    const productCategorySelect = document.getElementById('productCategory');

    openFormButton.addEventListener('click', () => {
        overlay.style.display = 'block';
    });

    closeOverlay.addEventListener('click', () => {
        overlay.style.display = 'none';
    });

    // Fetch available categories and populate the dropdown
    fetch('http://localhost:63946/api/category/getAllCategory')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok.');
            }
            return response.json();
        })
        .then(categories => {
            categories.forEach(category => {
                const option = document.createElement('option');
                option.value = category.id; // Assuming category objects have an 'id' property
                option.textContent = category.name; // Assuming category objects have a 'name' property
                productCategorySelect.appendChild(option);
            });
        })
        .catch(error => {
            console.error('Error fetching categories:', error);
            // Handle error scenario, show error message, etc.
        });

    productForm.addEventListener('submit', (e) => {
        e.preventDefault(); // Prevent form submission

        const productId = document.getElementById('productId').value;
        const productName = document.getElementById('productName').value;
        const productDescription = document.getElementById('productDescription').value;
        const productPrice = document.getElementById('productPrice').value;
        const productCategory = document.getElementById('productCategory').value;


        const productData = {
            id: productId,
            name: productName,
            description: productDescription,
            price: productPrice,
            status: "Active",
            categoryId: productCategory
        };

        addNewProduct(productData);
    });

    function addNewProduct(productData) {
        fetch('http://localhost:63946/api/product/addNewProduct', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                // Add any other headers as needed
            },
            body: JSON.stringify(productData),
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok.');
                }
                window.location.reload();
            })
            .catch(error => {
                console.error('Error adding product:', error);
                // Handle error scenario, show error message, etc.
            })
            .finally(() => {
                overlay.style.display = 'none'; // Hide the overlay after request completes
            });
    }
})