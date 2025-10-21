// Tab functionality
document.querySelectorAll('.tab-btn').forEach(button => {
    button.addEventListener('click', () => {
        const targetTab = button.dataset.tab;
        
        // Remove active class from all tabs and buttons
        document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));
        document.querySelectorAll('.tab-content').forEach(content => content.classList.remove('active'));
        
        // Add active class to clicked button and corresponding content
        button.classList.add('active');
        document.getElementById(targetTab).classList.add('active');
    });
});

// Initialize Telegram
document.getElementById('initBtn').addEventListener('click', async () => {
    const btn = document.getElementById('initBtn');
    const status = document.getElementById('initStatus');
    const codeSection = document.getElementById('codeInputSection');
    
    btn.disabled = true;
    btn.innerHTML = '🔄 Подключение...';
    status.className = 'status info';
    status.textContent = 'Подключение к Telegram...';
    
    try {
        const response = await fetch('/api/init', { method: 'POST' });
        const data = await response.json();
        
        if (data.needsCode) {
            status.className = 'status info';
            status.textContent = '💬 Код подтверждения отправлен в Telegram. Проверьте сообщения!';
            codeSection.style.display = 'block';
            
            // Start polling for status
            checkLoginStatus();
        } else if (data.message.includes('Error')) {
            status.className = 'status error';
            status.textContent = data.message;
            btn.disabled = false;
            btn.innerHTML = '🔐 Войти в Telegram';
        } else {
            status.className = 'status info';
            status.textContent = data.message;
            setTimeout(checkLoginStatus, 2000);
        }
    } catch (error) {
        status.className = 'status error';
        status.textContent = '❌ Ошибка подключения: ' + error.message;
        btn.disabled = false;
        btn.innerHTML = '🔐 Войти в Telegram';
    }
});

// Submit verification code
document.getElementById('submitCodeBtn').addEventListener('click', async () => {
    const code = document.getElementById('verificationCode').value;
    const status = document.getElementById('initStatus');
    
    if (!code) {
        alert('Введите код!');
        return;
    }
    
    try {
        await fetch('/api/verify-code', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ code })
        });
        
        status.className = 'status info';
        status.textContent = '⚙️ Проверка кода...';
        
        // Continue checking status
        setTimeout(checkLoginStatus, 2000);
    } catch (error) {
        status.className = 'status error';
        status.textContent = '❌ Ошибка: ' + error.message;
    }
});

// Submit password for 2FA
document.getElementById('submitPasswordBtn').addEventListener('click', async () => {
    const password = document.getElementById('cloudPassword').value;
    const status = document.getElementById('initStatus');
    
    if (!password) {
        alert('Введите пароль!');
        return;
    }
    
    try {
        await fetch('/api/verify-password', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ password })
        });
        
        status.className = 'status info';
        status.textContent = '⚙️ Проверка пароля...';
        
        // Continue checking status
        setTimeout(checkLoginStatus, 2000);
    } catch (error) {
        status.className = 'status error';
        status.textContent = '❌ Ошибка: ' + error.message;
    }
});

// Check login status
async function checkLoginStatus() {
    const status = document.getElementById('initStatus');
    const btn = document.getElementById('initBtn');
    const codeSection = document.getElementById('codeInputSection');
    const passwordSection = document.getElementById('passwordInputSection');
    
    try {
        const response = await fetch('/api/check-status');
        const data = await response.json();
        
        if (data.isInitialized) {
            // Login complete
            codeSection.style.display = 'none';
            passwordSection.style.display = 'none';
            if (data.loginResult && data.loginResult.includes('Error')) {
                status.className = 'status error';
                status.textContent = '❌ ' + data.loginResult;
                btn.disabled = false;
                btn.innerHTML = '🔐 Войти в Telegram';
            } else {
                status.className = 'status success';
                status.textContent = '✅ ' + (data.loginResult || 'Авторизация завершена!');
                btn.innerHTML = '✅ Подключено';
            }
        } else if (data.waitingForPassword) {
            // Waiting for 2FA password
            codeSection.style.display = 'none';
            passwordSection.style.display = 'block';
            status.className = 'status info';
            status.textContent = '🔐 Требуется пароль облачного хранилища (2FA)';
            setTimeout(checkLoginStatus, 1000);
        } else if (data.waitingForCode) {
            // Still waiting for code
            setTimeout(checkLoginStatus, 1000);
        } else {
            // Still initializing
            setTimeout(checkLoginStatus, 1000);
        }
    } catch (error) {
        console.error('Status check error:', error);
        setTimeout(checkLoginStatus, 2000);
    }
}

// Send single message
document.getElementById('sendForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const result = document.getElementById('result1');
    const btn = e.target.querySelector('button[type="submit"]');
    
    btn.disabled = true;
    btn.innerHTML = '⏳ Отправка...';
    
    const username = document.getElementById('username1').value;
    const messageText = document.getElementById('message1').value;
    
    try {
        const response = await fetch('/api/send-message', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, messageText })
        });
        
        const data = await response.json();
        
        if (data.message.includes('Error')) {
            result.className = 'result error';
            result.textContent = '❌ ' + data.message;
        } else {
            result.className = 'result success';
            result.textContent = '✅ ' + data.message;
            e.target.reset();
        }
    } catch (error) {
        result.className = 'result error';
        result.textContent = '❌ Ошибка: ' + error.message;
    } finally {
        btn.disabled = false;
        btn.innerHTML = '📤 Отправить';
    }
});

// Send multiple messages
document.getElementById('multipleForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const result = document.getElementById('result2');
    const btn = e.target.querySelector('button[type="submit"]');
    
    btn.disabled = true;
    btn.innerHTML = '⏳ Отправка...';
    
    const username = document.getElementById('username2').value;
    const messageText = document.getElementById('message2').value;
    const count = parseInt(document.getElementById('count2').value);
    
    try {
        const response = await fetch('/api/send-multiple', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, messageText, count })
        });
        
        const data = await response.json();
        
        if (data.message.includes('Error')) {
            result.className = 'result error';
            result.textContent = '❌ ' + data.message;
        } else {
            result.className = 'result success';
            result.textContent = '✅ ' + data.message;
            e.target.reset();
        }
    } catch (error) {
        result.className = 'result error';
        result.textContent = '❌ Ошибка: ' + error.message;
    } finally {
        btn.disabled = false;
        btn.innerHTML = '📤 Отправить';
    }
});

// Send scheduled message
document.getElementById('scheduledForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const result = document.getElementById('result3');
    const btn = e.target.querySelector('button[type="submit"]');
    
    btn.disabled = true;
    btn.innerHTML = '⏳ Планирование...';
    
    const username = document.getElementById('username3').value;
    const messageText = document.getElementById('message3').value;
    const minutes = parseInt(document.getElementById('minutes3').value);
    
    try {
        const response = await fetch('/api/send-scheduled', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, messageText, minutes })
        });
        
        const data = await response.json();
        
        if (data.message.includes('Error')) {
            result.className = 'result error';
            result.textContent = '❌ ' + data.message;
        } else {
            result.className = 'result success';
            result.textContent = '✅ ' + data.message;
            e.target.reset();
        }
    } catch (error) {
        result.className = 'result error';
        result.textContent = '❌ Ошибка: ' + error.message;
    } finally {
        btn.disabled = false;
        btn.innerHTML = '⏰ Запланировать';
    }
});

// Send multiple scheduled messages
document.getElementById('scheduledMultipleForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const result = document.getElementById('result4');
    const btn = e.target.querySelector('button[type="submit"]');
    
    btn.disabled = true;
    btn.innerHTML = '⏳ Планирование...';
    
    const username = document.getElementById('username4').value;
    const messageText = document.getElementById('message4').value;
    const count = parseInt(document.getElementById('count4').value);
    const intervalMinutes = parseInt(document.getElementById('interval4').value);
    const delayHours = parseInt(document.getElementById('delayHours4').value);
    const delayMinutes = parseInt(document.getElementById('delayMinutes4').value);
    
    try {
        const response = await fetch('/api/send-multiple-scheduled', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, messageText, count, intervalMinutes, delayHours, delayMinutes })
        });
        
        const data = await response.json();
        
        if (data.message.includes('Error')) {
            result.className = 'result error';
            result.textContent = '❌ ' + data.message;
        } else {
            result.className = 'result success';
            result.textContent = '✅ ' + data.message;
            e.target.reset();
        }
    } catch (error) {
        result.className = 'result error';
        result.textContent = '❌ Ошибка: ' + error.message;
    } finally {
        btn.disabled = false;
        btn.innerHTML = '⏰ Запланировать';
    }
});
