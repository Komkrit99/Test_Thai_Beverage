<template>
  <div class="exam-container">
    <div class="exam-header">
      <h2>เพิ่มข้อสอบ</h2>
    </div>
    
    <div class="exam-content">
      <form @submit.prevent="saveExam" class="exam-form">
        <div class="form-group">
          <label for="question">คำถาม</label>
          <input 
            type="text" 
            id="question" 
            v-model="form.questionText" 
            required 
            class="form-input"
          />
        </div>
        
        <div class="form-group">
          <label for="option1">คำตอบ 1</label>
          <input 
            type="text" 
            id="option1" 
            v-model="form.option1" 
            required 
            class="form-input"
          />
        </div>
        
        <div class="form-group">
          <label for="option2">คำตอบ 2</label>
          <input 
            type="text" 
            id="option2" 
            v-model="form.option2" 
            required 
            class="form-input"
          />
        </div>
        
        <div class="form-group">
          <label for="option3">คำตอบ 3</label>
          <input 
            type="text" 
            id="option3" 
            v-model="form.option3" 
            required 
            class="form-input"
          />
        </div>
        
        <div class="form-group">
          <label for="option4">คำตอบ 4</label>
          <input 
            type="text" 
            id="option4" 
            v-model="form.option4" 
            required 
            class="form-input"
          />
        </div>
        
        <div class="form-group">
          <label for="correctAnswer">คำตอบที่ถูกต้อง (1-4)</label>
          <select 
            id="correctAnswer" 
            v-model="form.correctAnswer" 
            required 
            class="form-input"
          >
            <option value="">เลือกคำตอบที่ถูกต้อง</option>
            <option value="1">คำตอบ 1</option>
            <option value="2">คำตอบ 2</option>
            <option value="3">คำตอบ 3</option>
            <option value="4">คำตอบ 4</option>
          </select>
        </div>
        
        <div v-if="error" class="error">{{ error }}</div>
        
        <div class="form-buttons">
          <button 
            type="submit" 
            class="save-btn" 
            :disabled="saving"
          >
            {{ saving ? 'กำลังบันทึก...' : 'บันทึก' }}
          </button>
          <button 
            type="button" 
            class="cancel-btn" 
            @click="cancelForm"
            :disabled="saving"
          >
            ยกเลิก
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const saving = ref(false)
const error = ref('')

const form = ref({
  questionText: '',
  option1: '',
  option2: '',
  option3: '',
  option4: '',
  correctAnswer: ''
})

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080'

const saveExam = async () => {
  saving.value = true
  error.value = ''
  
  try {
    const response = await fetch(`${API_BASE_URL}/api/ExamQuestions`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        questionText: form.value.questionText,
        option1: form.value.option1,
        option2: form.value.option2,
        option3: form.value.option3,
        option4: form.value.option4,
        correctAnswer: parseInt(form.value.correctAnswer)
      })
    })
    
    if (!response.ok) {
      const errorData = await response.json()
      throw new Error(errorData.message || 'Failed to save exam question')
    }
    
    // Navigate back to exam list
    router.push('/exams')
  } catch (err) {
    error.value = 'เกิดข้อผิดพลาดในการบันทึกข้อสอบ: ' + err.message
  } finally {
    saving.value = false
  }
}

const cancelForm = () => {
  router.push('/exams')
}
</script>

<style scoped>
.exam-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
  font-family: Arial, sans-serif;
}

.exam-header {
  background-color: #28a745;
  color: white;
  padding: 15px;
  text-align: center;
  margin-bottom: 20px;
}

.exam-header h2 {
  margin: 0;
  font-size: 18px;
}

.exam-content {
  background-color: #f8f9fa;
  padding: 30px;
  border: 2px solid #28a745;
}

.exam-form {
  max-width: 600px;
  margin: 0 auto;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
  font-size: 14px;
}

.form-input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 14px;
  box-sizing: border-box;
}

.form-input:focus {
  outline: none;
  border-color: #28a745;
  box-shadow: 0 0 0 2px rgba(40, 167, 69, 0.2);
}

.error {
  color: #dc3545;
  margin-bottom: 20px;
  padding: 10px;
  background-color: #f8d7da;
  border: 1px solid #f5c6cb;
  border-radius: 4px;
  text-align: center;
}

.form-buttons {
  display: flex;
  justify-content: center;
  gap: 15px;
  margin-top: 30px;
}

.save-btn {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  min-width: 100px;
}

.save-btn:hover:not(:disabled) {
  background-color: #0056b3;
}

.save-btn:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.cancel-btn {
  background-color: #dc3545;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  min-width: 100px;
}

.cancel-btn:hover:not(:disabled) {
  background-color: #c82333;
}

.cancel-btn:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}
</style>