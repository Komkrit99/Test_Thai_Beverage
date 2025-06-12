import { createRouter, createWebHistory } from 'vue-router'
import ExamListView from '../views/ExamListView.vue'
import AddExamView from '../views/AddExamView.vue'
import TestExamView from '../views/TestExamView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/exams'
    },
    {
      path: '/exams',
      name: 'exams',
      component: ExamListView,
    },
    {
      path: '/exams/add',
      name: 'add-exam',
      component: AddExamView,
    },
    {
      path: '/exams/test',
      name: 'test-exam',
      component: TestExamView,
    }
  ],
})

export default router
