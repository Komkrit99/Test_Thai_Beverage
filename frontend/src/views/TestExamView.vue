<template>
  <div class="exam-container">
    <div class="exam-header">
      <h2>ทำข้อสอบ</h2>
    </div>
    
    <div class="exam-content">
      <div v-if="loading" class="loading">กำลังโหลดข้อสอบ...</div>
      
      <div v-else-if="error" class="error">{{ error }}</div>
      
      <div v-else-if="!showResults" class="exam-taking">
        <div class="exam-info">
          <p><strong>จำนวนข้อสอบ:</strong> {{ questions.length }} ข้อ</p>
          <p><strong>เวลาที่เริ่มทำ:</strong> {{ startTime }}</p>
        </div>
        
        <div class="questions-container">
          <div v-for="(question, index) in questions" :key="question.id" class="question-item">
            <div class="question-header">
              <span class="question-number">{{ question.questionNumber }}. {{ question.questionText }}</span>
            </div>
            
            <div class="options">
              <label class="option" v-for="optionNum in [1, 2, 3, 4]" :key="optionNum">
                <input 
                  type="radio" 
                  :name="`question-${question.id}`" 
                  :value="optionNum"
                  v-model="userAnswers[question.id]"
                  class="option-radio"
                />
                <span class="option-text">{{ question[`option${optionNum}`] }}</span>
              </label>
            </div>
          </div>
        </div>
        
        <div class="submit-section">
          <button 
            class="submit-btn" 
            @click="submitExam"
            :disabled="!allQuestionsAnswered"
          >
            ส่งข้อสอบ
          </button>
          <p v-if="!allQuestionsAnswered" class="warning">
            กรุณาตอบคำถามให้ครบทุกข้อก่อนส่งข้อสอบ
          </p>
        </div>
      </div>
      
      <div v-else class="exam-results">
        <div class="results-header">
          <h3>ผลการสอบ</h3>
          <div class="score-summary">
            <div class="score-circle" :class="scoreClass">
              <span class="score-text">{{ score }}/{{ questions.length }}</span>
              <span class="percentage">{{ percentage }}%</span>
            </div>
          </div>
        </div>
        
        <div class="exam-info">
          <p><strong>เวลาที่เริ่มทำ:</strong> {{ startTime }}</p>
          <p><strong>เวลาที่ส่งข้อสอบ:</strong> {{ endTime }}</p>
          <p><strong>เวลาที่ใช้:</strong> {{ timeTaken }}</p>
        </div>
        
        <div class="detailed-results">
          <h4>รายละเอียดคำตอบ</h4>
          <div v-for="(question, index) in questions" :key="question.id" class="result-item">
            <div class="question-header">
              <span class="question-number">{{ question.questionNumber }}. {{ question.questionText }}</span>
              <span class="result-status" :class="isCorrect(question) ? 'correct' : 'incorrect'">
                {{ isCorrect(question) ? '✓ ถูก' : '✗ ผิด' }}
              </span>
            </div>
            
            <div class="options-result">
              <div v-for="optionNum in [1, 2, 3, 4]" :key="optionNum" 
                   class="option-result" 
                   :class="getOptionClass(question, optionNum)">
                <span class="option-indicator">{{ optionNum }}.</span>
                <span class="option-text">{{ question[`option${optionNum}`] }}</span>
                <span v-if="optionNum === question.correctAnswer" class="correct-indicator">✓ คำตอบที่ถูก</span>
                <span v-if="optionNum === userAnswers[question.id]" class="user-answer-indicator">
                  {{ optionNum === question.correctAnswer ? '' : '← คำตอบของคุณ' }}
                </span>
              </div>
            </div>
          </div>
        </div>
        
        <div class="action-buttons">
          <button class="retry-btn" @click="retakeExam">ทำข้อสอบใหม่</button>
          <button class="back-btn" @click="goBack">กลับไปหน้าข้อสอบ</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const questions = ref([])
const userAnswers = ref({})
const loading = ref(false)
const error = ref('')
const showResults = ref(false)
const startTime = ref('')
const endTime = ref('')
const examStartTime = ref(null)
const examEndTime = ref(null)

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
    
    userAnswers.value = {}
    data.forEach(question => {
      userAnswers.value[question.id] = null
    })
    
    examStartTime.value = new Date()
    startTime.value = examStartTime.value.toLocaleString('th-TH')
    
  } catch (err) {
    error.value = 'เกิดข้อผิดพลาดในการโหลดข้อสอบ: ' + err.message
  } finally {
    loading.value = false
  }
}

const allQuestionsAnswered = computed(() => {
  return questions.value.every(question => userAnswers.value[question.id] !== null)
})

const score = computed(() => {
  return questions.value.filter(question => 
    userAnswers.value[question.id] === question.correctAnswer
  ).length
})

const percentage = computed(() => {
  if (questions.value.length === 0) return 0
  return Math.round((score.value / questions.value.length) * 100)
})

const scoreClass = computed(() => {
  const percent = percentage.value
  if (percent >= 80) return 'excellent'
  if (percent >= 70) return 'good'
  if (percent >= 60) return 'fair'
  return 'poor'
})

const timeTaken = computed(() => {
  if (!examStartTime.value || !examEndTime.value) return ''
  
  const diff = examEndTime.value - examStartTime.value
  const minutes = Math.floor(diff / 60000)
  const seconds = Math.floor((diff % 60000) / 1000)
  
  return `${minutes} นาที ${seconds} วินาที`
})

const submitExam = () => {
  if (!allQuestionsAnswered.value) {
    alert('กรุณาตอบคำถามให้ครบทุกข้อก่อนส่งข้อสอบ')
    return
  }
  
  if (confirm('คุณต้องการส่งข้อสอบหรือไม่? หลังจากส่งแล้วจะไม่สามารถแก้ไขได้')) {
    examEndTime.value = new Date()
    endTime.value = examEndTime.value.toLocaleString('th-TH')
    showResults.value = true
  }
}

const isCorrect = (question) => {
  return userAnswers.value[question.id] === question.correctAnswer
}

const getOptionClass = (question, optionNum) => {
  const isUserAnswer = userAnswers.value[question.id] === optionNum
  const isCorrectAnswer = question.correctAnswer === optionNum
  
  if (isCorrectAnswer && isUserAnswer) return 'correct-user-answer'
  if (isCorrectAnswer) return 'correct-answer'
  if (isUserAnswer) return 'incorrect-user-answer'
  return ''
}

const retakeExam = () => {
  showResults.value = false
  userAnswers.value = {}
  questions.value.forEach(question => {
    userAnswers.value[question.id] = null
  })
  examStartTime.value = new Date()
  startTime.value = examStartTime.value.toLocaleString('th-TH')
  examEndTime.value = null
  endTime.value = ''
}

const goBack = () => {
  router.push('/exams')
}

onMounted(() => {
  fetchQuestions()
})
</script>

<style scoped>
.exam-container {
  max-width: 900px;
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

.loading, .error {
  text-align: center;
  padding: 20px;
  font-size: 16px;
}

.error {
  color: #dc3545;
}

.exam-info {
  background-color: #e9ecef;
  padding: 15px;
  margin-bottom: 20px;
  border-radius: 5px;
}

.exam-info p {
  margin: 5px 0;
}

.questions-container {
  margin-bottom: 30px;
}

.question-item {
  margin-bottom: 30px;
  padding: 20px;
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.question-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
}

.question-number {
  font-weight: bold;
  font-size: 16px;
}

.options {
  margin-left: 20px;
}

.option {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
  cursor: pointer;
  padding: 8px;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.option:hover {
  background-color: #f8f9fa;
}

.option-radio {
  margin-right: 10px;
}

.option-text {
  font-size: 14px;
}

.submit-section {
  text-align: center;
  padding: 20px;
  background-color: white;
  border-radius: 8px;
}

.submit-btn {
  background-color: #28a745;
  color: white;
  border: none;
  padding: 12px 30px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 16px;
  font-weight: bold;
}

.submit-btn:hover:not(:disabled) {
  background-color: #218838;
}

.submit-btn:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.warning {
  color: #dc3545;
  margin-top: 10px;
  font-size: 14px;
}

/* Results Styles */
.results-header {
  text-align: center;
  margin-bottom: 30px;
}

.results-header h3 {
  margin-bottom: 20px;
  font-size: 24px;
  color: #28a745;
}

.score-summary {
  display: flex;
  justify-content: center;
  margin-bottom: 20px;
}

.score-circle {
  width: 120px;
  height: 120px;
  border-radius: 50%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: bold;
}

.score-circle.excellent {
  background-color: #28a745;
}

.score-circle.good {
  background-color: #17a2b8;
}

.score-circle.fair {
  background-color: #ffc107;
  color: #212529;
}

.score-circle.poor {
  background-color: #dc3545;
}

.score-text {
  font-size: 24px;
}

.percentage {
  font-size: 16px;
}

.detailed-results {
  margin-top: 30px;
}

.detailed-results h4 {
  margin-bottom: 20px;
  color: #495057;
}

.result-item {
  margin-bottom: 25px;
  padding: 20px;
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.result-item .question-header {
  margin-bottom: 15px;
}

.result-status {
  font-weight: bold;
  padding: 4px 8px;
  border-radius: 4px;
}

.result-status.correct {
  background-color: #d4edda;
  color: #155724;
}

.result-status.incorrect {
  background-color: #f8d7da;
  color: #721c24;
}

.options-result {
  margin-left: 20px;
}

.option-result {
  display: flex;
  align-items: center;
  margin-bottom: 8px;
  padding: 8px;
  border-radius: 4px;
}

.option-indicator {
  margin-right: 10px;
  font-weight: bold;
}

.correct-answer {
  background-color: #d4edda;
  color: #155724;
}

.incorrect-user-answer {
  background-color: #f8d7da;
  color: #721c24;
}

.correct-user-answer {
  background-color: #d1ecf1;
  color: #0c5460;
}

.correct-indicator, .user-answer-indicator {
  margin-left: auto;
  font-size: 12px;
  font-weight: bold;
}

.correct-indicator {
  color: #28a745;
}

.user-answer-indicator {
  color: #dc3545;
}

.action-buttons {
  text-align: center;
  margin-top: 30px;
  padding: 20px;
}

.retry-btn, .back-btn {
  margin: 0 10px;
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
}

.retry-btn {
  background-color: #17a2b8;
  color: white;
}

.retry-btn:hover {
  background-color: #138496;
}

.back-btn {
  background-color: #6c757d;
  color: white;
}

.back-btn:hover {
  background-color: #5a6268;
}
</style>