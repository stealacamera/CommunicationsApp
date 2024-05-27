function addUserQueryFunctionality(userSearchInput, usersQueryResultDiv) {
    userSearchInput.addEventListener('keyup', e => {
        const isKeyAlphanumeric = e.keyCode >= 48 && e.keyCode <= 90;

        if (!isKeyAlphanumeric && e.keyCode != 8)
            return;

        for (let i = usersQueryResultDiv.children.length - 1; i >= 0; i--) {
            const memberDiv = usersQueryResultDiv.children[i];

            if (!memberDiv.querySelector('input[name=memberIds]:checked'))
                usersQueryResultDiv.removeChild(memberDiv);
        }

        if (!userSearchInput.value)
            return;

        $.ajax({
            url: `users/query?query=${userSearchInput.value}`,
            type: 'POST',
            success: data => {
                data.forEach(member => {
                    const memberCheckbox = document.createElement('input');
                    memberCheckbox.id = `member-${member.id}`;

                    if (usersQueryResultDiv.querySelector(`#${memberCheckbox.id}`))
                        return;

                    memberCheckbox.type = 'checkbox';
                    memberCheckbox.name = 'memberIds';
                    memberCheckbox.value = member.id;
                    memberCheckbox.classList.add('form-check-input');

                    const label = document.createElement('label');
                    label.classList.add('form-check-label');
                    label.setAttribute('for', memberCheckbox.id);
                    label.innerHTML = `<span style="font-weight: bold">${member.userName}</span> ${member.email}`

                    const memberDiv = document.createElement('div');
                    memberDiv.appendChild(memberCheckbox);
                    memberDiv.appendChild(label);
                    memberDiv.classList.add('form-check');

                    usersQueryResultDiv.appendChild(memberDiv);
                });
            },
            error: () => 
                Toastify({
                    text: "Something went wrong with your request. Please try again later",
                    duration: 3000,
                    close: true,
                    gravity: "bottom",
                    position: "right",
                }).showToast()
        });
    });
}