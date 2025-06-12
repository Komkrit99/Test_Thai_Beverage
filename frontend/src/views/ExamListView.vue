<template>
  <div class="exam-container">
    <div class="exam-header">
      <h2>ข้อสอบ</h2>
    </div>
    
    <div class="exam-content">
      <div class="action-buttons">
        <button class="add-exam-btn" @click="goToAddExam">เพิ่มข้อสอบ</button>
        <button class="test-exam-btn" @click="goToTestExam" :disabled="questions.length === 0">ทำข้อสอบ</button>
      </div>
      
      <div v-if="loading" class="loading">กำลังโหลด...</div>
      
      <div v-else-if="error" class="error">{{ error }}</div>
      
      <div v-else class="questions-list">
        <div v-for="question in questions" :key="question.id" class="question-item">
          <div class="question-header">
            <span class="question-number">{{ question.questionNumber }}. {{ question.questionText }}</span>
            <button class="delete-btn" @click="deleteQuestion(question.id)">ลบ</button>
          </div>
          
          <div class="options">
            <div class="option">
              <span class="option-circle"></span>
              <span>{{ question.option1 }}</span>
            </div>
            <div class="option">
              <span class="option-circle"></span>
              <span>{{ question.option2 }}</span>
            </div>
            <div class="option">
              <span class="option-circle"></span>
              <span>{{ question.option3 }}</span>
            </div>
            <div class="option">
              <span class="option-circle"></span>
              <span>{{ question.option4 }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const questions = ref([])
const loading = ref(false)
const error = ref('')

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080'

const fetchQuestions = async () => {
  loading.value = true
  error.value = ''
  
  try {
    const response = await fetch(`${API_BASE_URL}/api/ExamQuestions`)
    if (!response.ok) {
      throw new Error('Failed to fetch questions')
    }
    
    const data = await response.json()
    questions.value = data
  } catch (err) {
    error.value = 'เกิดข้อผิดพลาดในการโหลดข้อสอบ: ' + err.message
  } finally {
    loading.value = false
  }
}

const deleteQuestion = async (id) => {
  if (!confirm('คุณต้องการลบข้อสอบนี้หรือไม่?')) {
    return
  }
  
  try {
    const response = await fetch(`${API_BASE_URL}/api/ExamQuestions/${id}`, {
      method: 'DELETE'
    })
    
    if (!response.ok) {
      throw new Error('Failed to delete question')
    }
    
    await fetchQuestions()
  } catch (err) {
    error.value = 'เกิดข้อผิดพลาดในการลบข้อสอบ: ' + err.message
  }
}

const goToAddExam = () => {
  router.push('/exams/add')
}

const goToTestExam = () => {
  router.push('/exams/test')
}

onMounted(() => {
  fetchQuestions()
})
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
  padding: 20px;
  border: 2px solid #28a745;
}

.action-buttons {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
}

.add-exam-btn, .test-exam-btn {
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  color: white;
}

.add-exam-btn {
  background-color: #28a745;
}

.add-exam-btn:hover {
  background-color: #218838;
}

.test-exam-btn {
  background-color: #007bff;
}

.test-exam-btn:hover:not(:disabled) {
  background-color: #0056b3;
}

.test-exam-btn:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.loading, .error {
  text-align: center;
  padding: 20px;
  font-size: 16px;
}

.error {
  color: #dc3545;
}

.questions-list {
  space-y: 20px;
}

.question-item {
  margin-bottom: 30px;
}

.question-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.question-number {
  font-weight: bold;
  font-size: 16px;
}

.delete-btn {
  background-color: #dc3545;
  color: white;
  border: none;
  padding: 4px 8px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
}

.delete-btn:hover {
  background-color: #c82333;
}

.options {
  margin-left: 20px;
}

.option {
  display: flex;
  align-items: center;
  margin-bottom: 8px;
  font-size: 14px;
}

.option-circle {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background-color: #6c757d;
  margin-right: 10px;
  flex-shrink: 0;
}
</style>