
"use strict";
class ProjectUserManager {
    constructor() {
        // Temel değişkenler
        this.projectId = new URLSearchParams(window.location.search).get('id');
        this.pageSize = 5;
        this.currentProjectPage = 1;
        this.currentAvailablePage = 1;

        // DOM elementleri
        this.projectUsersList = document.getElementById('projectUsersList');
        this.availableUsersList = document.getElementById('availableUsersList');
        this.projectUserSearch = document.getElementById('projectUserSearch');
        this.availableUserSearch = document.getElementById('availableUserSearch'); 

        // Yükleme göstergeleri
        this.projectUsersLoading = document.getElementById('projectUsersLoading');
        this.availableUsersLoading = document.getElementById('availableUsersLoading');
        this.projectUsersEmpty = document.getElementById('projectUsersEmpty');
        this.availableUsersEmpty = document.getElementById('availableUsersEmpty');

        // Sayfalama elementleri
        this.projectUsersPagination = document.getElementById('projectUsersPagination');
        this.availableUsersPagination = document.getElementById('availableUsersPagination');
        this.projectUsersPaginationInfo = document.getElementById('projectUsersPaginationInfo');
        this.availableUsersPaginationInfo = document.getElementById('availableUsersPaginationInfo');

        // Bildirim element
        this.notification = document.getElementById('notification');

        // Olay dinleyicilerini başlat
        this.initEventListeners();

        // İlk verileri yükle
        this.loadProjectUsers();
        this.loadAvailableUsers();
    }

    // Olay dinleyicilerini tanımla
    initEventListeners() {
        // Arama filtreleri için gecikme zamanlayıcıları
        let projectSearchTimer;
        let availableSearchTimer;

        // Projedeki kullanıcılar için arama
        this.projectUserSearch.addEventListener('input', (e) => {
            clearTimeout(projectSearchTimer);
            projectSearchTimer = setTimeout(() => {
                this.currentProjectPage = 1;
                this.loadProjectUsers();
            }, 300);
        });

        // Eklenebilir kullanıcılar için arama
        this.availableUserSearch.addEventListener('input', (e) => {
            clearTimeout(availableSearchTimer);
            availableSearchTimer = setTimeout(() => {
                this.currentAvailablePage = 1;
                this.loadAvailableUsers();
            }, 300);
        });

         

        // Dinamik olarak eklenen butonlar için delegasyon
        document.addEventListener('click', (e) => {
            // Kullanıcı çıkarma butonu
            if (e.target.classList.contains('btn-remove')) {
                const userId = e.target.getAttribute('data-user-id');
                this.removeUserFromProject(userId);
            }

            // Kullanıcı ekleme butonu
            if (e.target.classList.contains('btn-add')) {
                const userId = e.target.getAttribute('data-user-id');
                this.addUserToProject(userId);
            }

            // Projedeki kullanıcılar sayfalama butonu
            if (e.target.classList.contains('project-page')) {
                this.currentProjectPage = parseInt(e.target.getAttribute('data-page'));
                this.loadProjectUsers();
            }

            // Eklenebilir kullanıcılar sayfalama butonu
            if (e.target.classList.contains('available-page')) {
                this.currentAvailablePage = parseInt(e.target.getAttribute('data-page'));
                this.loadAvailableUsers();
            }
        });
    }

    loadProjectUsers() {
        this.showLoading(this.projectUsersList, this.projectUsersLoading);

        const searchTerm = this.projectUserSearch.value; 

        fetch(`get-user-project-list?projectId=${this.projectId}&pageNumber=${this.currentProjectPage}&pageSize=${this.pageSize}&search=${searchTerm}`)
            .then(response => response.json())
            .then(data => { 
                this.hideLoading(this.projectUsersLoading);

                // Veri yoksa boş durum mesajını göster
                if (data.data.projectUsersResponseDto.users.length === 0) {
                    this.projectUsersList.innerHTML = '';
                    this.projectUsersEmpty.style.display = 'block';
                    return;
                }

                this.projectUsersEmpty.style.display = 'none';
                this.renderProjectUsers(data.data.projectUsersResponseDto.users);
                this.updateProjectPagination(data.data.projectUsersResponseDto.totalCount);
            })
            .catch(error => {
                console.error('Kullanıcılar yüklenirken hata oluştu:', error);
                this.hideLoading(this.projectUsersLoading);
                this.showNotification('Kullanıcılar yüklenirken bir hata oluştu', 'error');
            });
    }

    // Eklenebilir kullanıcıları yükle
    loadAvailableUsers() {
        // Yükleme durumunu göster
        this.showLoading(this.availableUsersList, this.availableUsersLoading);

        // Arama parametresi
        const searchTerm = this.availableUserSearch.value;

        // API çağrısı - Projede olmayan kullanıcıları getir
        fetch(`get-available-users?projectId=${this.projectId}&pageNumber=${this.currentAvailablePage}&pageSize=${this.pageSize}&search=${searchTerm}`)
            .then(response => response.json())
            .then(data => { 
                this.hideLoading(this.availableUsersLoading);
               
                // Veri yoksa boş durum mesajını göster
                if (data.data[0].userResponses.length === 0) {
                    this.availableUsersList.innerHTML = '';
                    this.availableUsersEmpty.style.display = 'block';
                    return;
                }

                this.availableUsersEmpty.style.display = 'none';
                this.renderAvailableUsers(data.data[0].userResponses);
                this.updateAvailablePagination(data.data[0].totalCount);
            })
            .catch(error => {
                console.error('Kullanıcılar yüklenirken hata oluştu:', error);
                this.hideLoading(this.availableUsersLoading);
                this.showNotification('Kullanıcılar yüklenirken bir hata oluştu', 'error');
            });
    }

    // Projedeki kullanıcı listesini ekrana çiz
    renderProjectUsers(users) { 
        this.projectUsersList.innerHTML = '';

        users.forEach(user => {
            const userCard = document.createElement('div');
            userCard.classList.add('user-card');
            userCard.id = `project-user-${user.id}`;

            userCard.innerHTML = `
                        <div class="user-avatar">${user.fullName.charAt(0)}</div>
                        <div class="user-info">
                            <div class="user-name">${user.fullName}</div>
                            <div class="user-email">${user.email}</div> 
                        </div>
                        <button class="btn-remove" data-user-id="${user.id}">Çıkar</button>
                    `;

            this.projectUsersList.appendChild(userCard);
        });
    }

    // Eklenebilir kullanıcı listesini ekrana çiz
    renderAvailableUsers(users) {
        this.availableUsersList.innerHTML = ''; 
        users.forEach(user => {
            const userCard = document.createElement('div');
            userCard.classList.add('user-card');
            userCard.id = `available-user-${user.id}`;

            userCard.innerHTML = `
                        <div class="user-avatar">${user.fullName.charAt(0)}</div>
                        <div class="user-info">
                            <div class="user-name">${user.fullName}</div>
                            <div class="user-email">${user.email}</div> 
                        </div>
                        <button class="btn-add" data-user-id="${user.id}">Ekle</button>
                    `;

            this.availableUsersList.appendChild(userCard);
        });
    }

    // Kullanıcıyı projeden çıkar
    removeUserFromProject(userId) {
        Swal.fire({
            title: 'Kullanıcıyı Çıkar',
            text: 'Bu kullanıcıyı projeden çıkarmak istediğinize emin misiniz?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Evet, Çıkar',
            cancelButtonText: 'Vazgeç',
            confirmButtonColor: '#f1416c',
            cancelButtonColor: '#7e8299'
        }).then((result) => {
            if (result.isConfirmed) {
                // Kullanıcı kartını devre dışı bırak
                const userCard = document.getElementById(`project-user-${userId}`);
                if (userCard) {
                    userCard.style.opacity = '0.5';
                    userCard.style.pointerEvents = 'none';
                }

                // API çağrısı
                fetch('delete-user-project', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ UserId: userId, ProjectId: this.projectId })
                })
                    .then(response => {

                        if (!response.ok) {
                            return response.json().then(err => {
                                this.showNotification(err.Message, 'error');

                            });
                        }
                        return response.json();
                    })
                    .then(data => {
                        // Başarılı işlem
                        this.showNotification('Kullanıcı projeden çıkarıldı');

                        // Kullanıcı kartını animasyonla kaldır
                        if (userCard) {
                            userCard.style.height = `${userCard.offsetHeight}px`;
                            setTimeout(() => {
                                userCard.style.height = '0';
                                userCard.style.margin = '0';
                                userCard.style.padding = '0';
                                userCard.style.overflow = 'hidden';

                                setTimeout(() => {
                                    // Listeleri yenile
                                    this.loadProjectUsers();
                                    this.loadAvailableUsers();
                                }, 300);
                            }, 100);
                        }
                    })
                    .catch(error => {
                         
                        if (userCard) {
                            userCard.style.opacity = '1';
                            userCard.style.pointerEvents = 'all';
                        }
                    });
            }
        });
    }

    // Kullanıcıyı projeye ekle
    addUserToProject(userId) {
        // Kullanıcı kartını devre dışı bırak
        const userCard = document.getElementById(`available-user-${userId}`);
        if (userCard) {
            userCard.style.opacity = '0.5';
            userCard.style.pointerEvents = 'none';
        }

        // API çağrısı
        fetch('add-user-project', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ projectId: parseInt(this.projectId), userId: userId })
        })
            .then(response => {
                 
                if (!response.ok) {
                    return response.json().then(err => { 
                        this.showNotification(err.Message, 'error');

                    });
                }
                return response.json();
            })
            .then(data => {
                if (!data.success) {
                    this.showNotification(data.message, 'error'); 
                }

                // Başarılı işlem
                this.showNotification(data.message);

                // Kullanıcı kartını animasyonla kaldır
                if (userCard) {
                    userCard.style.height = `${userCard.offsetHeight}px`;
                    setTimeout(() => {
                        userCard.style.height = '0';
                        userCard.style.margin = '0';
                        userCard.style.padding = '0';
                        userCard.style.overflow = 'hidden';

                        setTimeout(() => {
                            // Listeleri yenile
                            this.loadProjectUsers();
                            this.loadAvailableUsers();
                        }, 1000);
                    }, 500);
                }
            })
            .catch(() => {  
                if (userCard) {
                    userCard.style.opacity = '1';
                    userCard.style.pointerEvents = 'all';
                }
            });
    }

    // Proje kullanıcıları sayfalama güncellemesi
    updateProjectPagination(totalCount) {
        const totalPages = Math.ceil(totalCount / this.pageSize);

        // Sayfalama bilgi metnini güncelle
        this.projectUsersPaginationInfo.textContent = `Toplam ${totalCount} kullanıcıdan ${(this.currentProjectPage - 1) * this.pageSize + 1} - ${Math.min(this.currentProjectPage * this.pageSize, totalCount)} arası gösteriliyor`;

        // Sayfalama butonlarını oluştur
        this.projectUsersPagination.innerHTML = '';

        if (totalPages <= 1) return;

        // Sayfa butonları
        for (let i = 1; i <= totalPages; i++) {
            const pageBtn = document.createElement('button');
            pageBtn.classList.add('page-link', 'project-page');
            pageBtn.setAttribute('data-page', i);
            pageBtn.textContent = i;

            if (i === this.currentProjectPage) {
                pageBtn.classList.add('active');
            }

            this.projectUsersPagination.appendChild(pageBtn);
        }
    }

    // Eklenebilir kullanıcılar sayfalama güncellemesi
    updateAvailablePagination(totalCount) {
        const totalPages = Math.ceil(totalCount / this.pageSize);

        // Sayfalama bilgi metnini güncelle
        this.availableUsersPaginationInfo.textContent = `Toplam ${totalCount} kullanıcıdan ${(this.currentAvailablePage - 1) * this.pageSize + 1} - ${Math.min(this.currentAvailablePage * this.pageSize, totalCount)} arası gösteriliyor`;

        // Sayfalama butonlarını oluştur
        this.availableUsersPagination.innerHTML = '';

        if (totalPages <= 1) return;

        // Sayfa butonları
        for (let i = 1; i <= totalPages; i++) {
            const pageBtn = document.createElement('button');
            pageBtn.classList.add('page-link', 'available-page');
            pageBtn.setAttribute('data-page', i);
            pageBtn.textContent = i;

            if (i === this.currentAvailablePage) {
                pageBtn.classList.add('active');
            }

            this.availableUsersPagination.appendChild(pageBtn);
        }
    }

    // Yükleme göstergesini göster
    showLoading(contentElement, loadingElement) { 
        loadingElement.style.display = 'block';
    }

    // Yükleme göstergesini gizle
    hideLoading(loadingElement) {
        document.querySelectorAll('.user-card').forEach(card => {
            card.style.opacity = '1';
        });
        loadingElement.style.display = 'none';
    }

    // Bildirim göster
    showNotification(message, type = 'success') {
        this.notification.textContent = message;
        this.notification.className = 'notification';

        if (type === 'error') {
            this.notification.style.backgroundColor = '#f1416c';
        } else {
            this.notification.style.backgroundColor = '#50cd89';
        }

        // Bildirim göster
        setTimeout(() => {
            this.notification.classList.add('show');

            // 3 saniye sonra gizle
            setTimeout(() => {
                this.notification.classList.remove('show');
            }, 3000);
        }, 100);
    }
}

// Sayfa yüklendiğinde kullanıcı yöneticisini başlat
document.addEventListener('DOMContentLoaded', function () {
    const urlParams = new URLSearchParams(window.location.search);
    const projectId = urlParams.get('id');
    const projectUserManager = new ProjectUserManager(projectId); 
});

 