
function upload() {

    const fileUploadInput = document.querySelector('.file-uploader');
  
    /// Validations ///
  
    if (!fileUploadInput.value) {
      return;
    }
  
    // using index [0] to take the first file from the array
    const image = fileUploadInput.files[0];
  
    if (!image.type.includes('image')) {
      return alert('Only images are allowed!');
    }
  
    // check if size (in bytes) exceeds 10 MB
    if (image.size > 10_000_000) {
      return alert('Maximum upload size is 10MB!');
    }
  
    /// Display the image on the screen ///
  
    const fileReader = new FileReader();
    fileReader.readAsDataURL(image);
  
    fileReader.onload = (fileReaderEvent) => {
      const profilePicture = document.querySelector('.profile-picture');
      profilePicture.style.backgroundImage = `url(${fileReaderEvent.target.result})`;
    }
  
    // upload image to the server or the cloud
  }
  
var modal = document.getElementById("PostModal");
var p_modal = document.getElementById("P_Modal");

var noti = document.getElementById("SuccessNoti");

var txtTitle = document.getElementById("title").value;
var txtDescription = document.getElementById("description").value;
var txtPrice = document.getElementById("price").value;
var urlImage = document.getElementById("image").value;



document.getElementById("goPackageBtn").addEventListener("click", function () {
    
    window.location.href = '/Package/Index';
});
document.getElementById("closePackage").addEventListener("click", function () {
    p_modal.style.display = "none";
});

// Get the input element
var priceInput = document.getElementById('price');
var priceValidationMessage = document.getElementById('priceValidationMessage');


const closeModalBtn = document.getElementById('closeModalBtn');
const postBtn = document.getElementById('postBtn');


closeModalBtn.addEventListener('click', function () {
    modal.style.display = 'none';
    txtTitle = '';
    txtDescription = '';
    txtPrice = '';

});

postBtn.addEventListener('click', function () {
    var title = document.getElementById("title").value.trim();
    var description = document.getElementById("description").value.trim();
    var price = document.getElementById("price").value.trim();
    var imageFile = document.querySelector(".file-uploader").files[0];
    var customItemInput = document.getElementById('categoryCustom').value.trim();
    var categorySelect = document.getElementById('category').value;
    var isValid = true; 

    // Validate title
    if (title === '') {
        alert("Title is required.");
        //showCustomAlert("Title is required.");
        isValid = false;
    }

    // Validate description
    if (description === '') {
        alert("Description is required.");
        isValid = false;
    }

    // Validate price (must be a number)
    if (isNaN(price) || price === '') {
        alert("Price must be a valid number.");
        isValid = false;
    }

    // Validate image
    if (!imageFile) {
        alert("Image is required.");
        isValid = false;
    }

    // Validate selected item
    if (categorySelect === 'other' && customItemInput === '') {
        alert("Custom item is required.");
        isValid = false;
    } 
    var category = categorySelect;
    var isNew = false;
    if (categorySelect === 'other') {
        category = customItemInput;
        isNew = true;
    }


    // Proceed with form submission if all fields are valid
    if (isValid) {
        // Create FormData object
        var formData = new FormData();
        formData.append("Title", title);
        formData.append("Description", description);
        formData.append("Price", price);
        formData.append("File", imageFile);
        formData.append("Category", category);
        formData.append("IsNewCategory", isNew);

        // Send POST request to controller endpoint
        fetch("/Shop/Post", {
            method: "POST",
            body: formData
        })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Network response was not ok');
            }
        })
            .then(data => {
                if (data.success) {
                    // Close the modal
                    modal.style.display = 'none';
                    // Show success notification
                    noti.style.display = 'block';
                    setTimeout(function () {
                        noti.style.display = 'none';
                    }, 8000);
                    // Clear form fields
                    document.getElementById('title').value = '';
                    document.getElementById('description').value = '';
                    document.getElementById('price').value = '';
                    document.getElementById('category').value = 'other';
                    document.getElementById('categoryCustom').value = '';
                    document.querySelector('.file-uploader').value = '';
                    document.querySelector('.profile-picture').style.backgroundImage = '';
                }
            })
            .catch(error => {
                // Handle error
                console.error('Error:', error);
            });
    }
});

document.getElementById('category').addEventListener('change', function () {
    var select = this;
    var customItemDiv = document.getElementById('customItemDiv');
    var customItemInput = document.getElementById('categoryCustom');
    if (select.value === 'other') {
        customItemDiv.style.display = 'block';
    } else {
        customItemDiv.style.display = 'none';
        customItemInput.value = '';
    }
});

