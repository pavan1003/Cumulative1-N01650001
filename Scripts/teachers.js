function AddTeacher() {

    // Required Variables
    var teacherFname = document.getElementById("TeacherFname");
    var teacherLname = document.getElementById("TeacherLname");
    var employeeNumber = document.getElementById("EmployeeNumber");
    var employeeNumberRegEx = /(T)\d{3}$/;
    var hireDate = document.getElementById("HireDate");
    var salary = document.getElementById("Salary");

    // Remove previous validation highlighting
    var allFields = document.querySelectorAll('.form-control');
    allFields.forEach(function (element) {
        element.style.background = ""; // Reset background color
    });

    // Validate fields
    if (teacherFname.value === "") {
        highlightField(teacherFname);
        return false;
    }
    if (teacherLname.value === "") {
        highlightField(teacherLname);
        return false;
    }
    if (employeeNumber.value === "" || !employeeNumberRegEx.test(employeeNumber.value)) {
        highlightField(employeeNumber);
        return false;
    }
    if (hireDate.value === "" || new Date(hireDate.value) > new Date()) {
        highlightField(hireDate);
        return false;
    }
    if (salary.value === "" || salary.value < 0) {
        highlightField(salary);
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
    sendTeacherData(teacherData);

    return false;
}

function highlightField(field) {
    field.style.background = "red";
    field.focus();
}

function sendTeacherData(data) {
    // AJAX request to send teacher data to server
    var URL = "/Teacher/Create";
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
                responseText.innerHTML = "Error: " + rq.responseText;

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
    var URL = "/Teacher/Delete";
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
                console.log(rq.responseText)
            }
        }
    };
    rq.send(JSON.stringify(data));
}

