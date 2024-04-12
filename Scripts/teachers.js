
function highlightField(field, errorMessage) {
    field.style.background = "red";
    field.focus();

    // Display error message
    var errorContainer = field.parentElement.querySelector('.error-message');
    if (errorContainer) {
        errorContainer.innerHTML = errorMessage;
    }
}
function validateTeacherData() {
    var teacherFname = document.getElementById("TeacherFname");
    var teacherLname = document.getElementById("TeacherLname");
    var employeeNumber = document.getElementById("EmployeeNumber");
    var employeeNumberRegEx = /(T)\d{3}$/;
    var hireDate = document.getElementById("HireDate");
    var salary = document.getElementById("Salary");
    var allFields = document.querySelectorAll('.form-control');

    //reset vaildation
    allFields.forEach(function (element) {
        element.style.background = ""; // Reset background color
    });
    // Reset validation messages
    var errorMessages = document.querySelectorAll('.text-danger');
    errorMessages.forEach(function (element) {
        element.innerHTML = ""; // Clear existing error messages
    });

    // Validate fields
    if (teacherFname.value === "") {
        highlightField(teacherFname, "First name is required");
        return false;
    }
    if (teacherLname.value === "") {
        highlightField(teacherLname, "Last name is required");
        return false;
    }
    if (employeeNumber.value === "" || !employeeNumberRegEx.test(employeeNumber.value)) {
        highlightField(employeeNumber, "Employee number must start with 'T' followed by 3 digits");
        return false;
    }
    if (hireDate.value === "" || new Date(hireDate.value) > new Date()) {
        highlightField(hireDate, "Hire date cannot be empty and must be in the past");
        return false;
    }
    if (salary.value === "" || salary.value < 0) {
        highlightField(salary, "Salary cannot be empty and must be a positive value");
        return false;
    }

    // All fields are valid
    return true;
}
function AddTeacher() {
    // Required Variables
    var teacherFname = document.getElementById("TeacherFname");
    var teacherLname = document.getElementById("TeacherLname");
    var employeeNumber = document.getElementById("EmployeeNumber");
    var hireDate = document.getElementById("HireDate");
    var salary = document.getElementById("Salary");

    // Validate fields
    if (!validateTeacherData()) {
        return false;
    }

    // All fields are valid, proceed with form submission
    var teacherData = {
        "TeacherFname": teacherFname.value,
        "TeacherLname": teacherLname.value,
        "EmployeeNumber": employeeNumber.value,
        "HireDate": hireDate.value,
        "Salary": salary.value
    };

    // Send data to server
    addTeacherData(teacherData);

    return false;
}

function addTeacherData(data) {
    // AJAX request to send teacher data to server
    var URL = "/api/TeacherData/addTeacher/";
    var rq = new XMLHttpRequest();

    var responseText = document.getElementById("ResponseText");

    rq.open("POST", URL, true);
    rq.setRequestHeader("Content-Type", "application/json");
    rq.onreadystatechange = function () {
        if (rq.readyState == 4) {
            if (rq.status == 200) {
                // Success response
                window.location.href = "/Teacher/List";
            } else if (rq.status == 400) {
                // Bad request response (invalid data)
                responseText.innerHTML = "Error: " + JSON.parse(rq.response).Message;

            } else {
                // Other error responses
                responseText.innerHTML = "Error: Failed to add teacher. Status: " + rq.status
            }
        }
    };
    rq.send(JSON.stringify(data));
}

function DeleteTeacher(TeacherId) {
    var data = { "id": TeacherId };
    // AJAX request to send teacher data to server
    var URL = "/api/TeacherData/DeleteTeacher/" + TeacherId;
    var rq = new XMLHttpRequest();

    var responseText = document.getElementById("ResponseText");

    rq.open("POST", URL, true);
    rq.setRequestHeader("Content-Type", "application/json");
    rq.onreadystatechange = function () {
        if (rq.readyState == 4) {
            if (rq.status == 200) {
                // Success response
                window.location.href = "/Teacher/List";
            } else {
                // Other error responses
                responseText.innerHTML = "Error: Failed to Delete teacher. Status: " + rq.status
            }
        }
    };
    rq.send(JSON.stringify(data));
}

function UpdateTeacher(TeacherId) {

    // Required Variables
    var teacherFname = document.getElementById("TeacherFname");
    var teacherLname = document.getElementById("TeacherLname");
    var employeeNumber = document.getElementById("EmployeeNumber");
    var hireDate = document.getElementById("HireDate");
    var salary = document.getElementById("Salary");

    // Validate fields
    if (!validateTeacherData()) {
        return false;
    }

    // All fields are valid, proceed with form submission
    var teacherData = {
        "TeacherId": TeacherId,
        "TeacherFname": teacherFname.value,
        "TeacherLname": teacherLname.value,
        "EmployeeNumber": employeeNumber.value,
        "HireDate": hireDate.value,
        "Salary": salary.value
    };

    // Send data to server
    updateTeacherData(teacherData);

    return false;
}

function updateTeacherData(data) {
    // AJAX request to send teacher data to server
    var URL = "/api/TeacherData/UpdateTeacher/" + data.TeacherId;
    var rq = new XMLHttpRequest();

    var responseText = document.getElementById("ResponseText");

    rq.open("POST", URL, true);
    rq.setRequestHeader("Content-Type", "application/json");
    rq.onreadystatechange = function () {
        if (rq.readyState == 4) {
            if (rq.status == 200) {
                // Success response
                window.location.href = "/Teacher/show/" + data.TeacherId;
            } else if (rq.status == 400) {
                // Bad request response (invalid data)
                responseText.innerHTML = "Error: " + JSON.parse(rq.response).Message;

            } else {
                // Other error responses
                responseText.innerHTML = "Error: Failed to update teacher. Status: " + rq.status
            }
        }
    };
    rq.send(JSON.stringify(data));
}