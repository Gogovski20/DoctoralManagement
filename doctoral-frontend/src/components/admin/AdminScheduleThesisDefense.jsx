import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { studentService } from "../../api/studentService";

export default function AdminScheduleThesisDefense() {
  const navigate = useNavigate();

  const [projects, setProjects] = useState([]);
  const [mentors, setMentors] = useState([]);

  const [form, setForm] = useState({
    projectId: "",
    scheduledAt: "",
    room: "",
    committeeMemberIds: [],
  });

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  /* ---------------- LOAD DATA ---------------- */
  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [projectsData, mentorsData] = await Promise.all([
        studentService.getDefenseEligibleProjects(),
        studentService.getMentors(),
      ]);

      setProjects(projectsData || []);
      setMentors(mentorsData || []);
    } catch (err) {
      console.error(err);
      setError("Failed to load scheduling data.");
    }
  };

  /* ---------------- HANDLERS ---------------- */
  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleCommitteeChange = (e) => {
    const selected = Array.from(e.target.selectedOptions).map(
      (opt) => parseInt(opt.value)
    );

    setForm({ ...form, committeeMemberIds: selected });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    if (form.committeeMemberIds.length < 3) {
      setError("At least 3 committee members are required.");
      return;
    }

    const payload = {
      projectId: parseInt(form.projectId),
      scheduledAt: new Date(form.scheduledAt).toISOString(),
      room: form.room,
      committeeMemberIds: form.committeeMemberIds,
    };

    try {
      setLoading(true);
      await studentService.scheduleThesisDefense(payload);
      alert("Thesis defense scheduled successfully.");
      navigate("/admin/thesis-defenses");
    } catch (err) {
      console.error(err);
      setError(
        err?.response?.data?.message ||
          "Failed to schedule thesis defense."
      );
    } finally {
      setLoading(false);
    }
  };

  /* ---------------- UI ---------------- */
  return (
    <div style={{ padding: "2rem", maxWidth: "700px", margin: "0 auto" }}>
      <h1 style={{ fontSize: "1.8rem", fontWeight: "bold" }}>
        Schedule Thesis Defense
      </h1>

      <form onSubmit={handleSubmit} style={{ marginTop: "1.5rem" }}>
        {error && (
          <div style={{ color: "red", marginBottom: "1rem" }}>
            {error}
          </div>
        )}

        {/* PROJECT SELECT */}
        <FormSelect
          label="Doctoral Project"
          name="projectId"
          value={form.projectId}
          onChange={handleChange}
          required
        >
          <option value="">Select project</option>
          {projects.map((p) => (
            <option key={p.id} value={p.id}>
              {p.title} — {p.studentName}
            </option>
          ))}
        </FormSelect>

        {/* DATE */}
        <FormField
          label="Scheduled Date & Time"
          name="scheduledAt"
          type="datetime-local"
          value={form.scheduledAt}
          onChange={handleChange}
          required
        />

        {/* ROOM */}
        <FormField
          label="Room"
          name="room"
          value={form.room}
          onChange={handleChange}
          required
        />

        {/* COMMITTEE MULTI SELECT */}
        <FormSelect
          label="Committee Members (min 3)"
          multiple
          value={form.committeeMemberIds}
          onChange={handleCommitteeChange}
          required
          style={{ height: "160px" }}
        >
          {mentors.map((m) => (
            <option key={m.id} value={m.id}>
              {m.fullName}
            </option>
          ))}
        </FormSelect>

        <button
          type="submit"
          disabled={loading}
          style={{
            marginTop: "1.5rem",
            padding: "0.75rem 1.5rem",
            backgroundColor: "#0d9488",
            color: "white",
            border: "none",
            borderRadius: "0.375rem",
            cursor: "pointer",
          }}
        >
          {loading ? "Scheduling..." : "Schedule Defense"}
        </button>
      </form>
    </div>
  );
}

/* ---------------- REUSABLE COMPONENTS ---------------- */

function FormField({ label, ...props }) {
  return (
    <div style={{ marginBottom: "1rem" }}>
      <label style={{ display: "block", marginBottom: "0.25rem", fontWeight: 500 }}>
        {label}
      </label>
      <input
        {...props}
        style={{
          width: "100%",
          padding: "0.5rem",
          borderRadius: "0.375rem",
          border: "1px solid #d1d5db",
        }}
      />
    </div>
  );
}

function FormSelect({ label, children, ...props }) {
  return (
    <div style={{ marginBottom: "1rem" }}>
      <label style={{ display: "block", marginBottom: "0.25rem", fontWeight: 500 }}>
        {label}
      </label>
      <select
        {...props}
        style={{
          width: "100%",
          padding: "0.5rem",
          borderRadius: "0.375rem",
          border: "1px solid #d1d5db",
        }}
      >
        {children}
      </select>
    </div>
  );
}
