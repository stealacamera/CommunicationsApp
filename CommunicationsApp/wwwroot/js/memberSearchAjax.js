const memberSearchInput = document.getElementById('memberSearchInput');
const membersSearchResult = document.getElementById('membersSearchResult');

if (memberSearchInput && membersSearchResult) {
    memberSearchInput.addEventListener('keyup', e => {
        const isKeyAlphanumeric = e.keyCode >= 48 && e.keyCode <= 90;

        if (!isKeyAlphanumeric && e.keyCode != 8)
            return;

        for (let i = membersSearchResult.children.length - 1; i >= 0; i--) {
            const memberDiv = membersSearchResult.children[i];

            if (!memberDiv.querySelector('input[name=memberIds]:checked'))
                membersSearchResult.removeChild(memberDiv);
        }

        if (!memberSearchInput.value)
            return;

        $.ajax({
            url: `users/query?query=${memberSearchInput.value}`,
            type: 'POST',
            success: data => {

                data.forEach(member => {
                    const memberCheckbox = document.createElement('input');
                    memberCheckbox.id = `member-${member.id}`;

                    if (membersSearchResult.querySelector(`#${memberCheckbox.id}`))
                        return;

                    memberCheckbox.type = 'checkbox';
                    memberCheckbox.name = 'memberIds';
                    memberCheckbox.value = member.id;

                    const label = document.createElement('label');
                    label.setAttribute('for', memberCheckbox.id);
                    label.innerHTML = `<span style="font-weight: bold">${member.userName}</span> ${member.email}`

                    const memberDiv = document.createElement('div');
                    memberDiv.appendChild(memberCheckbox);
                    memberDiv.appendChild(label);

                    membersSearchResult.appendChild(memberDiv);
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